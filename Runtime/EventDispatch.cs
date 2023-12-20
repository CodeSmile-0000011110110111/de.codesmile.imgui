// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.IMGUI
{
	public sealed class GuiEventDispatch
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

		private GuiEventDispatch() {} // hidden paramless ctor

		/// <summary>
		///     Create an instance of a GUI event dispatcher.
		/// </summary>
		/// <param name="target">The receiver that will get On**Event methods called.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public GuiEventDispatch([NotNull] IGuiEvents target)
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

			var shouldUseEvent = m_Target.OnGuiEvent(m_Event, filteredEventType);
			if (shouldUseEvent == false)
				shouldUseEvent = DispatchEvent(filteredEventType);

			if (shouldUseEvent)
			{
				// called before event.Use() on purpose since Use() will change EventType to "Used"
				m_Target.OnWillUseEvent(m_Event);

				m_Event.Use();
			}

			ResetEventProperties();
		}

		private Boolean DispatchEvent(EventType eventType) => eventType switch
		{
			// ignored
			EventType.Ignore => false,
			EventType.Used => false,
			EventType.TouchStationary => false, // this never fires and is unused in UnityReferenceSource

			// key events
			EventType.KeyDown => DispatchKeyDownEvent(),
			EventType.KeyUp => DispatchKeyUpEvent(),

			// pointer events
			EventType.MouseDown => DispatchPointerDownEvent(),
			EventType.TouchDown => DispatchPointerDownEvent(),
			EventType.MouseUp => DispatchPointerUpEvent(),
			EventType.TouchUp => DispatchPointerUpEvent(),
			EventType.MouseMove => DispatchPointerMoveEvent(),
			EventType.MouseDrag => DispatchPointerDragEvent(),
			EventType.TouchMove => DispatchPointerDragEvent(), // there is no touch move, it's always a drag (even literally)

			// pointer enter/leave events
			EventType.MouseEnterWindow => DispatchPointerEnterEvent(),
			EventType.TouchEnter => DispatchPointerEnterEvent(),
			EventType.MouseLeaveWindow => DispatchPointerLeaveEvent(),
			EventType.TouchLeave => DispatchPointerLeaveEvent(),

			// special input
			EventType.ContextClick => DispatchContextClickEvent(),
			EventType.ScrollWheel => DispatchScrollWheelEvent(),

			// drag & drop events
			EventType.DragUpdated => DispatchDragUpdateEvent(),
			EventType.DragPerform => DispatchDragPerformEvent(),
			EventType.DragExited => DispatchDragCancelEvent(),

			// commands
			EventType.ValidateCommand => DispatchValidateCommandEvent(),
			EventType.ExecuteCommand => DispatchExecuteCommandEvent(),

			// layout & paint
			EventType.Layout => DispatchLayoutEvent(),
			EventType.Repaint => DispatchRepaintEvent(),

			// there should not be any unhandled event type
			_ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null),
		};

		private Boolean DispatchKeyDownEvent() => m_Event.keyCode != KeyCode.None
			? m_Target.OnKeyDownEvent(m_Event, m_Event.keyCode)
			: m_Target.OnKeyboardCharacterEvent(m_Event, m_Event.character);

		private Boolean DispatchKeyUpEvent() => m_Target.OnKeyUpEvent(m_Event, m_Event.keyCode);

		private Boolean DispatchPointerDownEvent() => m_Event.clickCount == 2
			? m_Target.OnDoubleClickEvent(m_Event)
			: m_Target.OnPointerDownEvent(m_Event, m_Event.MouseButton());

		private Boolean DispatchPointerUpEvent() => m_Target.OnPointerUpEvent(m_Event, m_Event.MouseButton());

		private Boolean DispatchPointerMoveEvent() =>
			m_Target.OnPointerMoveEvent(m_Event, m_Event.mousePosition, m_Event.delta);

		private Boolean DispatchPointerDragEvent() =>
			m_Target.OnPointerDragEvent(m_Event, m_Event.mousePosition, m_Event.delta);

		private Boolean DispatchPointerEnterEvent() =>
			m_Target.OnPointerEnterWindowEvent(m_Event, m_Event.mousePosition);

		private Boolean DispatchPointerLeaveEvent() =>
			m_Target.OnPointerLeaveWindowEvent(m_Event, m_Event.mousePosition);

		private Boolean DispatchDragUpdateEvent() => m_Target.OnDragUpdateEvent(m_Event, m_Event.mousePosition);
		private Boolean DispatchDragPerformEvent() => m_Target.OnDragPerformEvent(m_Event, m_Event.mousePosition);
		private Boolean DispatchDragCancelEvent() => m_Target.OnDragCancelEvent(m_Event, m_Event.mousePosition);
		private Boolean DispatchContextClickEvent() => m_Target.OnContextClickEvent(m_Event, m_Event.mousePosition);
		private Boolean DispatchScrollWheelEvent() => m_Target.OnScrollWheelEvent(m_Event, m_Event.delta);
		private Boolean DispatchValidateCommandEvent() => m_Target.OnValidateCommandEvent(m_Event, m_Event.Command());
		private Boolean DispatchExecuteCommandEvent() => m_Target.OnExecuteCommandEvent(m_Event, m_Event.Command());
		private Boolean DispatchLayoutEvent() => m_Target.OnLayoutEvent(m_Event);
		private Boolean DispatchRepaintEvent() => m_Target.OnRepaintEvent(m_Event);

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
