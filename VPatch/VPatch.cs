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
using System.Collections.Generic;
using System.IO;
using VPatch.Checksum;
using VPatch.Internal;

namespace VPatch
{
	/// <summary>
	/// Utility class for providing the use of VPatch algorithms on
	/// a set of streams for creating or applying differential updates.
	/// </summary>
	public class VPatch
	{
		/// <summary>
		/// Size of the blocks the patching algorithm will use to check
		/// for differences in a file.
		/// </summary>
		/// <remarks>
		/// Defaults to 64. Not presently changeable as it can cause issues.
		/// </remarks>
		public long BlockSize
		{
			get {
				return mBlockSize;
			}
			
			private set {
				mBlockSize = MakeMultipleOfTwo(value);
			}
		}
		long mBlockSize;
		
		/// <summary>
		/// Maximum matches per block. Larger numbers degrade performance as
		/// it results in more thorough checking of blocks for sameness.
		/// </summary>
		/// <remarks>
		/// Defaults to 500. Not presently changeable as it can cause issues.
		/// </remarks>
		public long MaximumMatches { get; private set; }
		
		/// <summary>
		/// Creates a new VPatch object, for creating and applying patches
		/// using file streams.
		/// </summary>
		public VPatch()
		{
			BlockSize = PatchGenerator.DefaultBlockSize;
			MaximumMatches = PatchGenerator.DefaultMaxMatches;
		}
		
		public PatchApplyResponse ApplyPatch(Stream oldVersionFile, Stream patch, IPatchInterpreter interpreter, IPatchProgress prog, Stream output)
		{
			if (!interpreter.Analyze(patch)) {
				return PatchApplyResponse.Failed;
			}
			
			return interpreter.Apply(oldVersionFile, patch, output, prog);
		}
		
		/// <summary>
		/// Takes a stream for an old version of a file, preparing a
		/// block-by-block patch to transform it in to the new version of a
		/// file. The given formatter is used to output the prepared data,
		/// which is finally written to the output stream.
		/// </summary>
		/// <param name="oldVersionFile">
		/// Seekable and readable stream for the old version of data.
		/// </param>
		/// <param name="newVersionFile">
		/// Seekable and readable stream for the new version of data.
		/// </param>
		/// <param name="formatter">
		/// Formatter to transform prepared patch data to a useful output.
		/// </param>
		/// <param name="output"></param>
		public void CreatePatch(Stream oldVersionFile, Stream newVersionFile, IPatchFormatter formatter, IPatchProgress prog, Stream output)
		{
			if (oldVersionFile == null)
				throw new NullReferenceException();
			if (newVersionFile == null)
				throw new NullReferenceException();
			if (output == null)
				throw new NullReferenceException();
			
			if (oldVersionFile.CanSeek == false)
				throw new NotSupportedException();
			if (newVersionFile.CanSeek == false)
				throw new NotSupportedException();
			
			oldVersionFile.Seek(0, SeekOrigin.Begin);
			newVersionFile.Seek(0, SeekOrigin.Begin);
			
			var fileInfo = new PatchFileInformation();
			
			fileInfo.SourceChecksum = MD5.Check(oldVersionFile);
			oldVersionFile.Seek(0, SeekOrigin.Begin);
			fileInfo.TargetChecksum = MD5.Check(newVersionFile);
			newVersionFile.Seek(0, SeekOrigin.Begin);
			
			var patchGenerator = new PatchGenerator(oldVersionFile, oldVersionFile.Length,
			                                       newVersionFile, newVersionFile.Length);
			patchGenerator.BlockSize = BlockSize;
			patchGenerator.MaximumMatches = MaximumMatches;
			
			List<SameBlock> sameBlocks = new List<SameBlock>();
			patchGenerator.Execute(sameBlocks, prog);
			
			if (formatter != null) formatter.FormatPatch(fileInfo, sameBlocks, newVersionFile, output);
			sameBlocks.Clear();
		}
		
		/// <summary>
		/// Ideally this would take an input and ensure it was a power of two,
		/// changing it in to the nearest power of two if it wasn't already;
		/// however the algorithm doesn't presently work and just returns the
		/// number placed in.
		/// </summary>
		/// <param name="input">Number to check as a multiple of two.</param>
		/// <returns>Input, as the closest power of two.</returns>
		long MakeMultipleOfTwo(long input)
		{
			/*long counter = 0;
			long accum = input;
			
			while (accum > 0) {
				counter++;
				accum >>= 1;
			}
			accum = 1;
			while (counter > 0) {
				accum <<= 1;
				counter--;
			}*/
			// TODO: Fix this.
			return input;
		}
	}
}
