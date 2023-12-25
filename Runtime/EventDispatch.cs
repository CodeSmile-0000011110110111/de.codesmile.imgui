// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodeSmile.IMGUI
{
	public sealed class EventDispatch
	{
		private readonly IEventReceiver m_Receiver;

		private Event m_Event;
		private Int32 m_ControlId;

		private EventDispatch() {} // hidden paramless ctor

		/// <summary>
		///     Create an instance of a GUI event dispatcher.
		/// </summary>
		/// <param name="receiver">The receiver that will get On**Event methods called.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public EventDispatch([NotNull] IEventReceiver receiver)
		{
			if (receiver == null)
				throw new ArgumentNullException(nameof(receiver));

			m_Receiver = receiver;
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
		public void DispatchCurrentEvent(Int32 controlId)
		{
			SetEventProperties(controlId);

			var filteredEventType = m_Event.GetTypeForControl(controlId);

			var shouldUseEvent = m_Receiver.OnGuiEvent(m_Event, filteredEventType);
			if (shouldUseEvent == false)
				shouldUseEvent = DispatchEvent(filteredEventType);

			if (shouldUseEvent)
			{
				// called before event.Use() on purpose since Use() will change EventType to "Used"
				m_Receiver.OnWillUseEvent(m_Event);

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
			? m_Receiver.OnKeyDownEvent(m_Event, m_Event.keyCode)
			: m_Receiver.OnKeyboardCharacterEvent(m_Event, m_Event.character);

		private Boolean DispatchKeyUpEvent() => m_Receiver.OnKeyUpEvent(m_Event, m_Event.keyCode);

		private Boolean DispatchPointerDownEvent() => m_Event.clickCount == 2
			? m_Receiver.OnDoubleClickEvent(m_Event)
			: m_Receiver.OnPointerDownEvent(m_Event, m_Event.MouseButton());

		private Boolean DispatchPointerUpEvent() => m_Receiver.OnPointerUpEvent(m_Event, m_Event.MouseButton());

		private Boolean DispatchPointerMoveEvent() =>
			m_Receiver.OnPointerMoveEvent(m_Event, m_Event.mousePosition, m_Event.delta);

		private Boolean DispatchPointerDragEvent() =>
			m_Receiver.OnPointerDragEvent(m_Event, m_Event.mousePosition, m_Event.delta);

		private Boolean DispatchPointerEnterEvent() =>
			m_Receiver.OnPointerEnterWindowEvent(m_Event, m_Event.mousePosition);

		private Boolean DispatchPointerLeaveEvent() =>
			m_Receiver.OnPointerLeaveWindowEvent(m_Event, m_Event.mousePosition);

		private Boolean DispatchDragUpdateEvent() => m_Receiver.OnDragUpdateEvent(m_Event, m_Event.mousePosition);
		private Boolean DispatchDragPerformEvent() => m_Receiver.OnDragPerformEvent(m_Event, m_Event.mousePosition);
		private Boolean DispatchDragCancelEvent() => m_Receiver.OnDragCancelEvent(m_Event, m_Event.mousePosition);
		private Boolean DispatchContextClickEvent() => m_Receiver.OnContextClickEvent(m_Event, m_Event.mousePosition);
		private Boolean DispatchScrollWheelEvent() => m_Receiver.OnScrollWheelEvent(m_Event, m_Event.delta);
		private Boolean DispatchValidateCommandEvent() => m_Receiver.OnValidateCommandEvent(m_Event, m_Event.Command());
		private Boolean DispatchExecuteCommandEvent() => m_Receiver.OnExecuteCommandEvent(m_Event, m_Event.Command());
		private Boolean DispatchLayoutEvent()
		{
			m_Receiver.OnLayoutEvent(m_Event);
			return false;
		}

		private Boolean DispatchRepaintEvent()
		{
			m_Receiver.OnRepaintEvent(m_Event);
			return false;
		}

		private void SetEventProperties(Int32 controlId)
		{
			m_ControlId = controlId;
			m_Event = Event.current;
		}

		private void ResetEventProperties()
		{
			m_ControlId = 0;
			m_Event = null;
		}
	}
}
