// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;

namespace CodeSmile.IMGUI
{
	public static class CharExtensions
	{
		public static Boolean IsPrintable(this Char character) => character >= 32;
	}
}
