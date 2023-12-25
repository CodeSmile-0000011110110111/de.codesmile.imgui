// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.IMGUI;
using System;
using UnityEditor;
using UnityEngine;

namespace CodeSmileEditor
{
	public abstract class HandleBase : IEventReceiver
	{
		private readonly SerializedObject m_Target;
		private readonly Transform m_Transform;
		private EventDispatch m_Dispatch;
		private Int32 m_ControlIdHint;

		public EventContext Context { get; private set; }
		public Int32 ControlId { get; private set; }
		public Int32 ControlIdHint
		{
			get => m_ControlIdHint != 0 ? m_ControlIdHint : GetHashCode();
			protected set => m_ControlIdHint = value;
		}
		public FocusType FocusType { get; protected set; }

		public Color Color { get; set; } = Color.white;

		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; } = Quaternion.identity;
		public Vector3 Scale { get; set; } = Vector3.one;
		public Matrix4x4 Matrix { get; private set; }

		public SerializedObject Target => m_Target;

		public Boolean IsHot => ControlId != 0 &&
		                        (GUIUtility.hotControl == ControlId || GUIUtility.keyboardControl == ControlId);

		public Single HandleSize => HandleUtility.GetHandleSize(Position);

		public Vector3 SnapMove => EditorSnapSettings.move;
		public float SnapRotate => EditorSnapSettings.rotate;
		public float SnapScale => EditorSnapSettings.scale;

		/* fields
		 * position, rotation, scale
		 * matrix
		 *
		 *
		 * tasks a handle needs to perform:
		 *
		 *
		 * handle input
		 * draw itself
		 *
		 * get a control Id
		 * specify if keyboard focus
		 *
		 * set and unset itself as hot control
		 * test if hot control
		 *
		 * optional: implement a context menu
		 *
		 * set GUI.changed when changed
		 * RecordObject if changed, then apply value(s)
		 *
		 * layout: update distance to mouse (AddControl)
		 *
		 * repaint itself if handle updates outside user input (HandleUtility.Repaint)
		 * raycast to handle from mouse with HandleUtility.GUIPointToWorldRay
		 *
		 */

		public HandleBase(SerializedObject target)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));

			if (target.targetObject is Component component)
				m_Transform = component.transform;
			if (target.targetObject is GameObject go)
				m_Transform = go.transform;

			if (m_Transform != null)
			{
				Position = m_Transform.position;
				Rotation = m_Transform.rotation;
				Scale = m_Transform.localScale;
			}

			m_Target = target;
		}

		public virtual void OnLayoutEvent(Event evt)
		{
			var distance = HandleUtility.DistanceToRectangle(Position, Rotation, HandleSize);
			HandleUtility.AddControl(ControlId, distance);
		}

		public virtual void OnSceneGUI()
		{
			Matrix = m_Transform.localToWorldMatrix;
			ControlId = GUIUtility.GetControlID(ControlIdHint, FocusType);
			Debug.Log($"ControlId: {ControlId}, hint: {ControlIdHint}");

			DispatchEvent(ControlId, EventContext.OnSceneGui);

			using (var scope = new Handles.DrawingScope())
			{
				Handles.matrix = Matrix;
				Handles.color = Color;
				DrawHandle();
			}
		}

		protected void DispatchEvent(Int32 controlId, EventContext eventContext = EventContext.OnGui)
		{
			m_Dispatch ??= new EventDispatch(this);

			SetEventDispatchProperties(controlId, eventContext);
			m_Dispatch.DispatchCurrentEvent(ControlId);

			using (var scope = new Handles.DrawingScope())
			{
				Handles.matrix = Matrix;
				Handles.color = Color;
				DrawHandle();
			}

			ResetEventDispatchProperties();
		}

		private void SetEventDispatchProperties(Int32 controlId, EventContext eventContext)
		{
			ControlId = controlId != 0 ? controlId : GetHashCode();
			Context = eventContext;
		}

		private void ResetEventDispatchProperties()
		{
			ControlId = 0;
			Context = EventContext.None;
		}

		protected abstract void DrawHandle();
	}
}
