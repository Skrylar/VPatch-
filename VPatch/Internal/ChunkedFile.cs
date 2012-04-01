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
using System.Threading;
using System.Linq;
using VPatch.Checksum;

namespace VPatch.Internal
{
	public class ChunkedFile
	{
		public FileChunk[] Chunks;
		public long ChunkCount
		{
			get {
				return Chunks.Length;
			}
		}
		
		public ChunkedFile(Stream fileStream, long fileSize, long chunkSize)
		{
			long chunkCount = fileSize / chunkSize;
			
			if (chunkCount < 1) {
				Chunks = null;
			} else {
				Chunks = new FileChunk[chunkCount];
				
				Console.WriteLine("Filesize of {0} gives {1} chunks.", fileSize, chunkCount);
				
				byte[] data = new byte[chunkSize];
				for (long i = 0; i < chunkCount; i++) {
					fileStream.Read(data, 0, (int)chunkSize);
					Chunks[i] = new FileChunk();
					Chunks[i].Offset = i * chunkSize;
					CalculateChecksum(data, 0, chunkSize, Chunks[i].Checksum);
				}
				data = null;
				
				Array.Sort(Chunks);
				
				/*foreach (var c in Chunks) {
					Console.WriteLine("Mem: {0} Chk: {1}", c.Offset, c.Checksum.ToString());
					Console.ReadLine();
				}*/
			}
		}
		
		// function:
		//   Searches sortedArray[first]..sortedArray[last] for key.
		// returns: index of the matching element if it finds key,
		//         otherwise  -(index where it could be inserted)-1.
		// parameters:
		//   sortedArray in  array of sorted (ascending) values.
		//   first, last in  lower and upper subscript bounds
		//   key         in  value to search for.
		// returns:
		//   index of key, or -insertion_position -1 if key is not
		//                 in the array. This value can easily be
		//                 transformed into the position to insert it.
		public bool Search(ChunkChecksum key, out long start)
		{
			start = -1;
			if (ChunkCount == 0) return false;
			
			int idx = Array.BinarySearch(Chunks, key);
			if (idx >= 0) {
				start = idx;
				return true;
			} else {
				return false;
			}
		}
		
		public void CalculateChecksum(byte[] data, long start, long size, ChunkChecksum K)
		{
			unsafe {
				fixed (byte *p = &data[start]) {
					var i = (UInt64*)p;
					K.V = i[0];
				}
			}
			
			//K.Adler32 = 0;
			K.Adler32 = Adler32.Check(1, data, start, size);
		}
	}
}
