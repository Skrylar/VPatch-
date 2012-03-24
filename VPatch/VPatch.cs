/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 7:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using VPatch.Checksum;
using VPatch.Internal;

namespace VPatch
{
	public class VPatch
	{
		/// <summary>
		/// Size of the blocks the patching algorithm will use to check
		/// for differences in a file.
		/// </summary>
		/// <remarks>
		/// Defaults to 64. Must be a multiple of two.
		/// </remarks>
		public long BlockSize
		{
			get {
				return mBlockSize;
			}
			
			set {
				mBlockSize = MakeMultipleOfTwo(value);
			}
		}
		long mBlockSize;
		
		/// <summary>
		/// Maximum matches per block. Larger numbers degrade performance as
		/// it results in more thorough checking of blocks for sameness.
		/// </summary>
		/// <remarks>
		/// Defaults to 500.
		/// </remarks>
		public long MaximumMatches { get; set; }
		
		public VPatch()
		{
			BlockSize = PatchGenerator.DefaultBlockSize;
			MaximumMatches = PatchGenerator.DefaultMaxMatches;
		}
		
		public void CreatePatch(Stream oldVersionFile, Stream newVersionFile, IPatchFormatter formatter, Stream output)
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
			patchGenerator.Execute(sameBlocks);
			
			formatter.FormatPatch(fileInfo, sameBlocks, output);
			sameBlocks.Clear();
		}
		
		long MakeMultipleOfTwo(long input)
		{
			long counter = 0;
			long accum = input;
			
			while (accum > 0) {
				counter++;
				accum >>= 1;
			}
			accum = 1;
			while (counter > 0) {
				accum <<= 1;
				counter--;
			}
			return accum;
		}
	}
}
