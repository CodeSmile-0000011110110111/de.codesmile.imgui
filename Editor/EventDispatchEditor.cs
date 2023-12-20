// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.IMGUI;
using System;
using UnityEditor;

namespace CodeSmileEditor
{
	public class EventDispatchEditor : Editor, IEventReceiver
	{
		private EventDispatch m_Dispatch;
		public EventContext EventContext { get; private set; }
		public Int32 ControlId { get; private set; }

		public void DispatchEvent(EventContext eventContext = EventContext.OnGui, Int32 controlId = 0)
		{
			m_Dispatch ??= new EventDispatch(this);

			SetEventDispatchProperties(eventContext, controlId);
			m_Dispatch.DispatchCurrentEvent(ControlId);
			ResetEventDispatchProperties();
		}

		private void SetEventDispatchProperties(EventContext eventContext, Int32 controlId)
		{
			EventContext = eventContext;
			ControlId = controlId != 0 ? controlId : GetHashCode();
		}

		private void ResetEventDispatchProperties()
		{
			EventContext = EventContext.None;
			ControlId = 0;
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
