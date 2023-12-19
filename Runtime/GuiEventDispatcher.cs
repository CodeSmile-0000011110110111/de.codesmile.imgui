// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.IMGUI
{
	public sealed class GuiEventDispatcher
	{
		private readonly IGuiEvents m_Target;

		private Int32 m_ControlId;
		private Event m_Event;

		/// <summary>
		///     The Id of the control for which events are currently being processed.
		///     Is 0 outside of event processing.
		/// </summary>
		public Int32 ControlId => m_ControlId;

		/// <summary>
		///     The Event.current instance currently being processed.
		///     Is null outside of event processing.
		/// </summary>
		public Event CurrentEvent => m_Event;

		private GuiEventDispatcher() {} // hidden paramless ctor

		/// <summary>
		///     Create an instance of a GUI event dispatcher.
		/// </summary>
		/// <param name="target">The receiver that will get On**Event methods called.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public GuiEventDispatcher([NotNull] IGuiEvents target)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			m_Target = target;
		}

		/// <summary>
		///     Processes events for a control and calls the corresponding CodeSmile.IMGUI.IGuiEventReceiver methods.
		///     Call this from IMGUI event handling callbacks such as OnGUI, OnSceneGUI, OnInspectorGUI, OnPreviewGUI,
		///     OnInteractivePreviewGUI, and probably more.
		/// </summary>
		/// <param name="controlId">
		///     The Id for the control. If you pass 0 (default) then controlId will be the
		///     IGuiEventReceiver's ```GetHashCode()``` value under the assumption that the receiver manages a single control.
		/// </param>
		public void ProcessCurrentEvent(Int32 controlId = 0)
		{
			SetEventProperties(controlId);

			var filteredEventType = m_Event.GetTypeForControl(controlId);
			m_Target.OnGuiEvent(m_Event, filteredEventType);

			if (m_Event.type != EventType.Used)
			{
				var shouldUseEvent = DispatchEventTypeToReceiver(filteredEventType);
				if (shouldUseEvent)
				{
					// called before event.Use() on purpose since Use() will change EventType to "Used"
					m_Target.OnWillUseEvent(m_Event);

					m_Event.Use();
				}
			}

			ResetEventProperties();
		}

		private Boolean DispatchEventTypeToReceiver(EventType eventType) => eventType switch
		{
			// ignored
			EventType.Ignore => false,
			EventType.Used => false,
			// key events
			EventType.KeyDown => m_Event.keyCode != KeyCode.None
				? m_Target.OnKeyDownEvent(m_Event, m_Event.keyCode)
				: m_Target.OnKeyboardCharacterEvent(m_Event, m_Event.character),
			EventType.KeyUp => m_Target.OnKeyUpEvent(m_Event, m_Event.keyCode),
			// mouse events
			EventType.MouseDown => m_Event.clickCount == 2
				? m_Target.OnMouseDoubleClickEvent(m_Event, m_Event.MouseButton())
				: m_Target.OnMouseDownEvent(m_Event, m_Event.MouseButton()),
			EventType.MouseUp => m_Target.OnMouseUpEvent(m_Event, m_Event.MouseButton()),
			EventType.MouseMove => m_Target.OnMouseMoveEvent(m_Event, m_Event.mousePosition, m_Event.delta),
			EventType.MouseDrag => m_Target.OnMouseDragEvent(m_Event, m_Event.mousePosition, m_Event.delta),
			EventType.ScrollWheel => m_Target.OnScrollWheelEvent(m_Event, m_Event.delta),
			// touch events
			EventType.TouchDown => m_Target.OnDragUpdateEvent(m_Event, m_Event.mousePosition),
			EventType.TouchUp => m_Target.OnDragUpdateEvent(m_Event, m_Event.mousePosition),
			EventType.TouchMove => m_Target.OnDragUpdateEvent(m_Event, m_Event.mousePosition),
			EventType.TouchEnter => m_Target.OnDragUpdateEvent(m_Event, m_Event.mousePosition),
			EventType.TouchLeave => m_Target.OnDragUpdateEvent(m_Event, m_Event.mousePosition),
			EventType.TouchStationary => m_Target.OnDragUpdateEvent(m_Event, m_Event.mousePosition),
			// drag & drop events
			EventType.DragUpdated => m_Target.OnDragUpdateEvent(m_Event, m_Event.mousePosition),
			EventType.DragPerform => m_Target.OnDragPerformEvent(m_Event, m_Event.mousePosition),
			EventType.DragExited => m_Target.OnDragCancelEvent(m_Event, m_Event.mousePosition),
			// special events
			EventType.ValidateCommand => DispatchValidateCommandToReceiver(),
			EventType.ExecuteCommand => DispatchExecuteCommandToReceiver(),
			EventType.ContextClick => false,
			EventType.MouseEnterWindow => m_Target.OnMouseEnterWindowEvent(m_Event),
			EventType.MouseLeaveWindow => m_Target.OnMouseLeaveWindowEvent(m_Event),
			// layout & paint
			EventType.Layout => false,
			EventType.Repaint => false,
			_ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
		};

		public Boolean DispatchValidateCommandToReceiver() => m_Event.GuiCommand() switch
		{
			GuiCommand.Copy => m_Target.OnValidateCopyCommand(m_Event),
			GuiCommand.Unknown => false,
		};

		public Boolean DispatchExecuteCommandToReceiver() => m_Event.GuiCommand() switch
		{
			GuiCommand.Copy => m_Target.OnExecuteCopyCommand(m_Event),
			GuiCommand.Unknown => false,
		};

		private void SetEventProperties(Int32 controlId)
		{
			m_ControlId = controlId != 0 ? controlId : m_Target.GetHashCode();
			m_Event = Event.current;
		}

		private void ResetEventProperties()
		{
			m_ControlId = 0;
			m_Event = null;
		}
	}
}
