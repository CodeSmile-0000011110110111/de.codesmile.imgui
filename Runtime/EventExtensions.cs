// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.IMGUI
{
	public static class EventExtensions
	{
		public static MouseButton MouseButton(this Event evt) => (MouseButton)evt.button;

	}
}
