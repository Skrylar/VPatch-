/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/27/2012
 * Time: 6:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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
