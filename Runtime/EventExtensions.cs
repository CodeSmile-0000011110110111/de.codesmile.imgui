// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.IMGUI
{
	public static class EventExtensions
	{
		public static MouseButton MouseButton(this Event evt) => (MouseButton)evt.button;

		public static GuiCommand GuiCommand(this Event evt)
		{
			return evt.commandName switch
			{
				"Copy" => IMGUI.GuiCommand.Copy,
				_ => IMGUI.GuiCommand.Unknown,
			};
		}

	}
}
