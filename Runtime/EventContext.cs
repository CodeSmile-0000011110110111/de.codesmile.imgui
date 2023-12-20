// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

namespace CodeSmile.IMGUI
{
	/// <summary>
	///     Specifies which On*GUI method an event was dispatched from.
	/// </summary>
	/// <seealso cref="">
	///     - <see cref="EventDispatch" />
	/// </seealso>
	public enum EventContext
	{
		/// <summary>
		/// Outside event processing.
		/// </summary>
		None,
		/// <summary>
		/// Event dispatched from a OnGUI message method.
		/// </summary>
		OnGui,
		/// <summary>
		/// Event dispatched from a OnSceneGUI message method.
		/// </summary>
		OnSceneGui,
		/// <summary>
		/// Event dispatched from a OnInspectorGUI message method.
		/// </summary>
		OnInspectorGui,
		/// <summary>
		/// Event dispatched from a OnPreviewGUI message method.
		/// </summary>
		OnPreviewGui,
		/// <summary>
		/// Event dispatched from a OnInteractivePreviewGUI message method.
		/// </summary>
		OnInteractivePreviewGui,
	}
}
