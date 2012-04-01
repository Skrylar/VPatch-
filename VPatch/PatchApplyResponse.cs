/*
 * ---------------------------------------------------------------------------
 *                            -=* VPatch *=-
 * ---------------------------------------------------------------------------
 *  Copyright (C) 2001-2005 Koen van de Sande / Van de Sande Productions
 * ---------------------------------------------------------------------------
 *  Website: http://www.tibed.net/vpatch
 * 
 *  This software is provided 'as-is', without any express or implied
 *  warranty.  In no event will the authors be held liable for any damages
 *  arising from the use of this software.
 * 
 *  Permission is granted to anyone to use this software for any purpose,
 *  including commercial applications, and to alter it and redistribute it
 *  freely, subject to the following restrictions:
 * 
 *  1. The origin of this software must not be misrepresented; you must not
 *     claim that you wrote the original software. If you use this software
 *     in a product, an acknowledgment in the product documentation would be
 *     appreciated but is not required.
 *  2. Altered source versions must be plainly marked as such, and must not be
 *     misrepresented as being the original software.
 *  3. This notice may not be removed or altered from any source distribution.
 * ---------------------------------------------------------------------------
 * Ported to C# 2012 Joshua Cearley
 */
using System;

namespace VPatch
{
	/// <summary>
	/// How a patch was applied to a file: if it was, and what went wrong
	/// if it wasn't.
	/// </summary>
	public enum PatchApplyResponse
	{
		/// <summary>
		/// Patch was successfully applied and verified.
		/// </summary>
		Ok,
		/// <summary>
		/// Patching was not required; the files had identical checksums.
		/// </summary>
		NotRequired,
		/// <summary>
		/// Patching was not done; the input file did not have the checksum
		/// required as specified by the patch file.
		/// </summary>
		WrongFile,
		/// <summary>
		/// Patching did not succeed, or the patched file did not match the
		/// checksum required as specified by the patch file.
		/// </summary>
		Failed
	}
}
