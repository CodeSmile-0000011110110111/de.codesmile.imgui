// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEngine;
using UnityEngine.UIElements;

namespace CodeSmile.IMGUI
{
	public static class EventExtensions
	{
		/// <summary>
		///     Maps a Event.button value to the MouseButton enum for ease of use & mind.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>The event's mouse button as CodeSmile.IMGUI.MouseButton enum.</returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.IMGUI.MouseButton" />
		/// </seealso>
		public static MouseButton MouseButton(this Event evt) => (MouseButton)evt.button;

		/// <summary>
		///     Maps a Event.commandName to the GuiCommand enum for ease of use & mind.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns>
		///     The event's commandName mapped to CodeSmile.IMGUI.GuiCommand enum. Is set to
		///     GuiCommand.Custom for any commandName that isn't mapped.
		/// </returns>
		/// <seealso cref="">
		///     - <see cref="CodeSmile.IMGUI.EventCommand" />
		/// </seealso>
		public static EventCommand Command(this Event evt) => evt.commandName switch
		{
			"Cut" => EventCommand.Cut,
			"Copy" => EventCommand.Copy,
			"Paste" => EventCommand.Paste,
			"Duplicate" => EventCommand.Duplicate,
			"Delete" => EventCommand.Delete,
			"SoftDelete" => EventCommand.SoftDelete,
			"Rename" => EventCommand.Rename,
			"Find" => EventCommand.Find,

			"SelectAll" => EventCommand.SelectAll,
			"DeselectAll" => EventCommand.DeselectAll,
			"InvertSelection" => EventCommand.InvertSelection,
			"SelectChildren" => EventCommand.SelectChildren,
			"SelectPrefabRoot" => EventCommand.SelectPrefabRoot,

			"UndoRedoPerformed" => EventCommand.UndoRedoPerformed,
			"OnLostFocus" => EventCommand.OnLostFocus,
			"ModifierKeysChanged" => EventCommand.ModifierKeysChanged,
			"FrameSelected" => EventCommand.FrameSelected,
			"FrameSelectedWithLock" => EventCommand.FrameSelectedWithLock,
			"SceneViewPickingEventCommand" => EventCommand.SceneViewPicking,
			"SetSceneViewMotionHotControl" => EventCommand.SetSceneViewMotionHotControl,
			_ => EventCommand.Custom,
		};
	}
}
