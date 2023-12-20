// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
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
	public interface IEventReceiver
	{
		/// <summary>
		///     Use this for classic event handling where you switch over the type.
		///     Avoid this unless you are comfortable with classic IMGUI event handling or need to handle a special
		///     case that may be difficult to achieve with the new event dispatch methods.
		/// </summary>
		/// <param name="currentEvent">The Event.current instance.</param>
		/// <param name="filteredEventType">The event type for the control.</param>
		public Boolean OnGuiEvent(Event evt, EventType filteredEventType) => false;

		public Boolean OnKeyDownEvent(Event evt, KeyCode keyCode) => false;
		public Boolean OnKeyUpEvent(Event evt, KeyCode keyCode) => false;

		/// <summary>
		///     Called when the user typed a character on the keyboard. Also called when the key repeats.
		/// </summary>
		/// <remarks>
		///     The event is called repeatedly when the key is held down, the repeat delay and repeat rate are OS settings
		///     and likely differ per user.
		/// </remarks>
		/// <remarks>
		///     Character events should only be used for text input. Use OnKeyDownEvent to decide upon an action
		///     for a given key. There is no "character up" event.
		/// </remarks>
		/// <remarks>
		///     Explanation: The physical location of a character depends on the locale. In some locales, the given character
		///     may not be available or require pressing an inconvenient key combination. Furthermore, the printed character
		///     depends on key modifiers. For example, testing for character 'f' would not work with CAPS LOCK on or while Shift
		///     is held down, unless you also checked for character 'F'.
		/// </remarks>
		/// <param name="character">The character with modifiers applied, eg as it would appear in a text application.</param>
		/// <returns>Return true to stop the event from being processed by other controls, eg calls event.Use().</returns>
		public Boolean OnKeyboardCharacterEvent(Event evt, Char character) => false;

		public Boolean OnPointerEnterWindowEvent(Event evt, Vector2 position) => false;
		public Boolean OnPointerLeaveWindowEvent(Event evt, Vector2 position) => false;

		/// <summary>
		///     Called when the left mouse button is double-clicked or the screen is double-tapped.
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public Boolean OnDoubleClickEvent(Event evt) => false;

		public Boolean OnContextClickEvent(Event evt, Vector2 position) => false;


		/// <summary>
		///     Called when processing an EventType.MouseDown.
		/// </summary>
		/// <param name="evt">Current event.</param>
		/// <param name="button">Indicates which button was pressed.</param>
		/// <returns>True to use (consume) this event, false otherwise.</returns>
		public Boolean OnPointerDownEvent(Event evt, MouseButton button) => false;
		public Boolean OnPointerUpEvent(Event evt, MouseButton button) => false;
		public Boolean OnPointerMoveEvent(Event evt, Vector2 position, Vector2 positionDelta) => false;
		public Boolean OnPointerDragEvent(Event evt, Vector2 position, Vector2 positionDelta) => false;
		public Boolean OnScrollWheelEvent(Event evt, Vector2 wheelDelta) => false;

		public Boolean OnDragUpdateEvent(Event evt, Vector2 position) => false;
		public Boolean OnDragPerformEvent(Event evt, Vector2 position) => false;
		public Boolean OnDragCancelEvent(Event evt, Vector2 position) => false;

		public Boolean OnValidateCommandEvent(Event evt, EventCommand command) => false;
		public Boolean OnExecuteCommandEvent(Event evt, EventCommand command) => false;

		public Boolean OnLayoutEvent(Event evt) => false;
		public Boolean OnRepaintEvent(Event evt) => false;

		/// <summary>
		///     If the event should be used this is called right before actually using the event.
		/// </summary>
		/// <param name="evt">
		///     The event that was processed and marked as "to be used". Its EventType will be the original type
		/// and not EventType.Used since Use() has not been called on the event yet.
		/// </param>
		public void OnWillUseEvent(Event evt) {}
	}
}
