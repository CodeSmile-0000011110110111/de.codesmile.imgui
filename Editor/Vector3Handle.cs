// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.IMGUI;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmileEditor
{
	public class Vector3Handle : HandleBase
	{
		public event Action<Vector3> onValueChanged;

		public Vector3Handle(SerializedObject target)
			: base(target) {}

		protected override void DrawHandle() => Handles.DrawWireCube(Vector3.zero, Vector3.one);

		// var t = (target as Component).transform
		// Handles.PositionHandle(position,)
		public void OnSceneGUI()
		{
			FocusType = FocusType.Keyboard;
			ControlIdHint = nameof(Vector3Handle).GetHashCode();
			Color = Color.red;
			base.OnSceneGUI();
			Debug.Log($"is hot: {IsHot}");
		}
	}

	public class FloatHandle : HandleBase, IEventReceiver
	{
		public event Action<Single> onValueChanged;

		private Single m_Value;

		public Single Value
		{
			get => m_Value;
			private set
			{
				m_Value = value;
				onValueChanged?.Invoke(value);
			}
		}

		public FloatHandle(SerializedObject target)
			: base(target) {}

		public Boolean OnPointerDownEvent(Event evt, MouseButton button)
		{
			if (HandleUtility.nearestControl != ControlId)
				return false;
			if (button != MouseButton.LeftMouse)
				return false;

			GUIUtility.hotControl = GUIUtility.keyboardControl = ControlId;
			return true;
		}

		public Boolean OnPointerUpEvent(Event evt, MouseButton button)
		{
			if (GUIUtility.hotControl != ControlId)
				return false;
			if (button != MouseButton.LeftMouse)
				return false;

			GUIUtility.hotControl = 0;
			return true;
		}

		protected override void DrawHandle()
		{
			Handles.DrawWireCube(Vector3.zero, Vector3.one);
			//Value = Handles.ScaleValueHandle(ControlId, Value, Position, Rotation, HandleUtility.GetHandleSize(Position), Handles.DotHandleCap, EditorSnapSettings.scale);
			Value = Handles.ScaleSlider(ControlId, Value, Position, Vector3.forward, Rotation, HandleSize, SnapScale);
			//Debug.Log($"value: {Value}");
		}

		public void OnSceneGUI()
		{
			FocusType = FocusType.Keyboard;
			ControlIdHint = nameof(Vector3Handle).GetHashCode();
			Color = Color.cyan;
			base.OnSceneGUI();
		}
	}
}
