// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.IMGUI
{
	/// <summary>
	///     Common Event.commandName strings mapped to an enum so we need not typo-check commandName strings.
	///     <remarks>
	///         The Custom type is used when the commandName string was not mapped. In that case comparing
	///         Event.commandName string is required.
	///     </remarks>
	/// </summary>
	public enum EventCommand
	{
		Custom,

		Cut, // Ctrl+X
		Copy, // Ctrl+C
		Paste, // Ctrl+V
		Duplicate, // Ctrl+D
		Delete, // Shift+Del
		SoftDelete, // Del
		Rename,
		Find, // Ctrl+F

		SelectAll, // Ctrl+A ??
		DeselectAll, //
		InvertSelection,
		SelectChildren,
		SelectPrefabRoot,

		UndoRedoPerformed,
		OnLostFocus,
		ModifierKeysChanged,
		FrameSelected, // 'F' in scene view
		FrameSelectedWithLock, // Shift+F in scene view
		SceneViewPicking, // LMB in scene view
		SetSceneViewMotionHotControl, // RMB in scene view
	}
}
