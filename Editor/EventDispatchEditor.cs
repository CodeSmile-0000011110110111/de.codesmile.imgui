// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.IMGUI;
using System;
using UnityEditor;

namespace CodeSmileEditor
{
	public class EventDispatchEditor : Editor
	{
		private GuiEventDispatcher m_EventDispatcher;
		public GuiEventDispatcher EventDispatcher => m_EventDispatcher;

		// tbd Input state
		// hashcode (control hint)
		// focus type

		// default control

		public void DispatchEvent(IGuiEvents receiver, Int32 controlId = 0)
		{
			// var currentEvent = Event.current;
			// var filteredEventType = currentEvent.GetTypeForControl(controlId);
			//
			// if (filteredEventType == EventType.Layout)
			// 	HandleUtility.AddDefaultControl(controlId);

			m_EventDispatcher ??= new GuiEventDispatcher(receiver);
			m_EventDispatcher.ProcessCurrentEvent(controlId);
		}

		// TODO: implement these in another class
		// if (IsNearestControl(controlId))
		// 	shouldUseEvent = TrySetHotControl(evt, controlId);
		// if (IsHotControl(controlId))
		// 	shouldUseEvent = TryUnsetHotControl(evt, evt.MouseButton());
		// TODO: move up one layer
		// private static Boolean IsNearestControl(Int32 controlId) => controlId == HandleUtility.nearestControl;
		// private static bool TrySetHotControl(Event evt, Int32 controlId)
		// {
		// 	// holding alt + LMB is alternative for RMB (rotate scene view)
		// 	if (IsAltPressed(evt))
		// 		return false;
		//
		// 	if (IsEventUsed(evt))
		// 		return false;
		//
		// 	if (evt.MouseButton() != MouseButton.Left)
		// 		return false;
		//
		// 	GUIUtility.hotControl = controlId;
		// 	GUIUtility.keyboardControl = controlId;
		// 	return true;
		//
		// }
		//
		// private static bool TryUnsetHotControl(Event evt, MouseButton mouseButton)
		// {
		// 	if (IsAltPressed(evt))
		// 		return false;
		//
		// 	if (IsEventUsed(evt))
		// 		return false;
		//
		// 	if (mouseButton != MouseButton.Left && mouseButton != MouseButton.Right)
		// 		return false;
		//
		// 	GUIUtility.hotControl = 0;
		// 	//GUIUtility.keyboardControl = 0;
		// 	return true;
		// }
		//
		// private static Boolean IsAltPressed(Event evt) => evt.alt;
		//
		// private static Boolean IsEventUsed(Event evt)
		// {
		// 	if (evt.type == EventType.Used)
		// 		return true;
		//
		// 	return false;
		// }
		//
		// private static Boolean IsHotControl(Int32 controlId) => controlId == GUIUtility.hotControl;
	}
}
