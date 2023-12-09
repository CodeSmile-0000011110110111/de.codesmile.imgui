// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.IMGUI
{
	/// <summary>
	///     Implement this to receive On**Event calls for any OnGUI/OnSceneGUI/etc and receive the Event.current instance by
	///     letting the Event get processed by an instance of the CodeSmile.IMGUI.GuiEventDispatch class.
	///     This makes it easier to write IMGUI event handling code compared to the classic C-style code that
	///     involves either ```switch (Event.current.type) {..}``` or comparable if/else if conditions.
	/// </summary>
	public interface IGuiEventReceiver
	{
		/// <summary>
		///     Called when processing an EventType.MouseDown.
		/// </summary>
		/// <param name="evt">Current event.</param>
		/// <param name="button">Indicates which button was pressed.</param>
		/// <returns>True to use (consume) this event, false otherwise.</returns>
		public Boolean OnMouseDownEvent(MouseButton button) => false;

		public Boolean OnMouseUpEvent(MouseButton button) => false;
		public Boolean OnMouseMoveEvent(Vector2 mousePosition) => false;

		public Boolean OnKeyDownEvent(KeyCode keyCode) => false;
		public Boolean OnKeyUpEvent(KeyCode keyCode) => false;

		/// <summary>
		///     Called when the user typed a character on the keyboard.
		/// </summary>
		/// <remarks>
		///     This should only be used for text input! Use OnKeyDownEvent to decide upon an action for a given key.
		/// </remarks>
		/// <remarks>
		///     Explanation: The physical location of a character depends on the locale. In some locales, the given character
		///     may not be available or require pressing an inconvenient key combination. Furthermore, the printed character
		///     depends on key modifiers. For example, testing for character 'f' would not work with CAPS LOCK on or while Shift
		///     is held down, unless you also checked for character 'F'.
		/// </remarks>
		/// <param name="character"></param>
		/// <returns></returns>
		public Boolean OnKeyboardCharacterEvent(Char character) => false;

		/// <summary>
		///     If the event should be used this is called right before actually using the event.
		/// </summary>
		/// <param name="evt">
		///     The event that was processed and marked as "to be used".
		///     The evt.type will be the original type and not EventType.Used.
		/// </param>
		public void OnBeforeUseEvent(Event evt) {}

		//public void OnUseEvent(Event evt, EventType eventTypeBeforeUse){}
	}

	public sealed class GuiEventDispatcher
	{
		private readonly IGuiEventReceiver m_Receiver;

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
		/// <param name="receiver">The receiver that will get On**Event methods called.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public GuiEventDispatcher([NotNull] IGuiEventReceiver receiver)
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
		public void ProcessEvent(Int32 controlId = 0)
		{
			SetEventProperties(controlId);

			var eventType = m_Event.GetTypeForControl(controlId);
			var shouldUseEvent = false;

			switch (eventType)
			{
				// don't do anything
				case EventType.Ignore: break;
				case EventType.Used: break;

				// mouse events
				case EventType.MouseDown:
					shouldUseEvent = m_Receiver.OnMouseDownEvent(m_Event.MouseButton());
					break;
				case EventType.MouseUp:
					shouldUseEvent = m_Receiver.OnMouseUpEvent(m_Event.MouseButton());
					break;
				case EventType.MouseMove:
					shouldUseEvent = m_Receiver.OnMouseMoveEvent(m_Event.mousePosition);
					break;
				case EventType.MouseDrag:
					break;
				case EventType.MouseEnterWindow:
					break;
				case EventType.MouseLeaveWindow:
					break;
				case EventType.ScrollWheel:
					break;

				// key events
				case EventType.KeyDown:
					var keyCode = m_Event.keyCode;
					if (keyCode != KeyCode.None)
						shouldUseEvent = m_Receiver.OnKeyDownEvent(keyCode);
					else
						shouldUseEvent = m_Receiver.OnKeyboardCharacterEvent(m_Event.character);
					break;
				case EventType.KeyUp:
					shouldUseEvent = m_Receiver.OnKeyUpEvent(m_Event.keyCode);
					break;

				// touch events
				case EventType.TouchDown:
					break;
				case EventType.TouchUp:
					break;
				case EventType.TouchMove:
					break;
				case EventType.TouchEnter:
					break;
				case EventType.TouchLeave:
					break;
				case EventType.TouchStationary:
					break;

				// drag events
				case EventType.DragUpdated:
					break;
				case EventType.DragPerform:
					break;
				case EventType.DragExited:
					break;

				// commands
				case EventType.ValidateCommand:
					break;
				case EventType.ExecuteCommand:
					break;
				case EventType.ContextClick:
					break;

				// layout & paint
				case EventType.Layout:
					break;
				case EventType.Repaint:
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}

			if (shouldUseEvent)
			{
				// must call this before using so that event.type is still the original type, not "Used"
				m_Receiver.OnBeforeUseEvent(m_Event);

				m_Event.Use();
			}

			ClearEventProperties();
		}

		private void SetEventProperties(Int32 controlId)
		{
			if (controlId == 0)
				controlId = m_Receiver.GetHashCode();

			m_ControlId = controlId;
			m_Event = Event.current;
		}

		private void ClearEventProperties()
		{
			m_ControlId = 0;
			m_Event = null;
		}
	}
}
