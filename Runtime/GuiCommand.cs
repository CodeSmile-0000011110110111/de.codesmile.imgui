// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.IMGUI
{
	public enum GuiCommand
	{
		Unknown = -1,

		Copy,
		Cut,
		Paste,
		Delete,// Shift+Del
		SoftDelete, // Del
		Duplicate,// Ctrl+D
		FrameSelected, // 'F' in scene view
		FrameSelectedWithLock, // Shift+F in scene view
		SelectAll,// Ctrl+A ??
		Find, // Ctrl+F

		FocusProjectWindow, // ??

		SceneViewPicking, //+EventCommand	LMB
		SetSceneViewMotionHotControl, //+EventCommand	RMB
	}
}
