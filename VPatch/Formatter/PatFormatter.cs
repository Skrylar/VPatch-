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
using System.Collections.Generic;
using VPatch.Internal;

namespace VPatch.Formatter
{
	/// <summary>
	/// Formats patches to VPatch 3.1's native format 
	/// </summary>
	public class PatFormatter : IPatchFormatter
	{
		public PatFormatter()
		{
		}
		
		const int COPY_BUF_SIZE = 4096;
		
		public void FormatPatch(PatchFileInformation fileInfo, IList<SameBlock> sameBlocks, Stream target, Stream output)
		{
			using (var bw = new BinaryWriter(output)) {
				#region Patch File Preface
				bw.Write((UInt32)0x54415056); // Write magic header
				UInt32 fileCount = 0x80000000; // MD5 mode. Top byte is for extensions.
				fileCount += 1; // We're only packing one file in.
				bw.Write((UInt32)fileCount); // Go ahead and put the file count in.
				#endregion
				
				#region Current File Preface
				long bodySize = 0;
				long noBlocks = 0;
				long noBlocksOffset = output.Position;
				bw.Write((UInt32)noBlocks);
				bw.Write(fileInfo.SourceChecksum, 0, 16);
				bw.Write(fileInfo.TargetChecksum, 0, 16);
				long bodySizeOffset = output.Position;
				bw.Write((UInt32)bodySize);
				#endregion
				
				byte[] copyBuffer = new byte[COPY_BUF_SIZE];
				for(int iter = 0; iter < sameBlocks.Count; iter++) {
					SameBlock current = sameBlocks[iter];
		
					// store current block
					if(current.Size > 0) {
						// copy block from sourceFile
						if(current.Size < 256) {
							bw.Write((byte)1);
							bw.Write((byte)current.Size);
							bodySize += 2;
						} else if(current.Size < 65536) {
							bw.Write((byte)2);
							bw.Write((UInt16)current.Size);
							bodySize += 3;
						} else {
							bw.Write((byte)3);
							bw.Write((UInt32)current.Size);
							bodySize += 5;
						}
						bw.Write((UInt32)current.SourceOffset);
						bodySize += 4;
						noBlocks++;
					}
					iter++;
					if(iter >= sameBlocks.Count) break;
					SameBlock next = sameBlocks[iter];
					iter--;
		
					// calculate area inbetween this block and the next
					long notFoundStart = current.TargetOffset+current.Size;
					if(notFoundStart > next.TargetOffset) {
						throw new InvalidOperationException("makeBinaryPatch input problem: there was overlap");
					}
					long notFoundSize = next.TargetOffset - notFoundStart;
					if(notFoundSize > 0) {
						// we need to include this area in the patch directly
						if(notFoundSize < 256) {
							bw.Write((byte)5);
							bw.Write((byte)notFoundSize);
							bodySize += 2;
						} else if(notFoundSize < 65536) {
							bw.Write((byte)6);
							bw.Write((UInt16)notFoundSize);
							bodySize += 3;
						} else {
							bw.Write((byte)7);
							bw.Write((UInt32)notFoundSize);
							bodySize += 5;
						}
						// copy from target...
						target.Seek(notFoundStart, SeekOrigin.Begin);
						for(long i = 0; i < notFoundSize; i += COPY_BUF_SIZE) {
							long j = notFoundSize - i;
							if(j > COPY_BUF_SIZE) j = COPY_BUF_SIZE;
							target.Read(copyBuffer, 0, (int)j);
							output.Write(copyBuffer, 0, (int)j);
						}
						bodySize += notFoundSize;
						noBlocks++;
					}
				}
		
				// we are done, now add just one extra block with the target file time
				bw.Write((byte)255);
				long time = fileInfo.TargetDateTime.ToBinary();
				
				bw.Write((Int64)time);
				
				noBlocks++;
				bodySize += 9;
		
				long curPos = output.Position;
				output.Seek(noBlocksOffset, SeekOrigin.Begin);
				bw.Write((UInt32)noBlocks);
				output.Seek(bodySizeOffset, SeekOrigin.Begin);
				bw.Write((UInt32)bodySize);
				output.Seek(curPos, SeekOrigin.Begin);
			}
		}
	}
}
