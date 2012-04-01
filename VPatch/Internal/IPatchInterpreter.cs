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
using System.IO;

namespace VPatch.Internal
{
	public interface IPatchInterpreter
	{
		/// <summary>
		/// Analyzes a patch stream to detect whether it can be intepreted by this
		/// patch interpreter; additionally, load neccesary header data to process
		/// a patch. After analysis the stream will be returned to the original
		/// read position.
		/// </summary>
		/// <param name="patchStream">
		/// Seekable and readable stream to analyze patch data from.
		/// </param>
		/// <returns>
		/// Whether the file was analyzed as a potentially viable patch file, and
		/// the relevant header information has been grabbed.
		/// </returns>
		bool Analyze(Stream patchStream);
		
		/// <summary>
		/// Applies a patch from the patch stream using information gathered
		/// from the <see cref="Analyze" /> step, transforming the oldVersion
		/// in to a new version using the given patchData and writing the
		/// newly updated file to the output stream.
		/// </summary>
		/// <param name="oldVersion">
		/// Seekable stream containing the old version of the file to be updated.
		/// </param>
		/// <param name="patchStream">
		/// Seekable stream containing the patch data, ostensibly the same
		/// stream as used in the analyze step.
		/// </param>
		/// <param name="output">
		/// Stream where the new version of the file will be written to, after
		/// performing any block updates as written in the patch data.
		/// </param>
		/// <param name="prog">
		/// Interface that will receive intermittent progress updates on the
		/// application of the patch. May be null if you don't want this
		/// information.
		/// </param>
		PatchApplyResponse Apply(Stream oldVersion, Stream patchStream, Stream output, IPatchProgress prog);
	}
}
