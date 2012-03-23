/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 5:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace VPatch
{
	public class ChunkedFile
	{
		public FileChunk[] Chunks { get; private set; }
		public int ChunkCount
		{
			get {
				return Chunks.Length;
			}
		}
		
		public ChunkedFile(Stream fileStream, int fileSize, int chunkSize)
		{
			int chunkCount = fileSize / chunkSize;
			
			if (chunkCount < 1) {
				Chunks = null;
			} else {
				Chunks = new FileChunk[chunkCount];
				
				byte[] data = new byte[chunkSize];
				for (int i = 0; i < chunkCount; i++) {
					fileStream.Read(data, 0, chunkSize);
					Chunks[i].Offset = i * chunkSize;
					CalculateChecksum(data, 0, chunkSize, ref Chunks[i].Checksum);
				}
				data = null;
				
				Array.Sort(Chunks);
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
		public bool Search(ChunkChecksum key, out int start)
		{
			start = -1;
			if (ChunkCount == 0) return false;
			int first = 0;
			int last = ChunkCount - 1;
			while (first <= last) {
				int mid = (first + last) / 2; // compute mid point
				if (key == Chunks[mid].Checksum) {
					while (true) {
						if (mid == 0) break;
						mid--;
						if (!(key == Chunks[mid].Checksum)) {
							mid++;
							break;
						}
					}
					start = mid;
					return true; // found it. return position
				}
				if (key < Chunks[mid].Checksum) {
					last = mid - 1; // repeat search in bottom half.
				} else {
					first = mid + 1; // repeat search in top half.
				}
			}
			return false;
		}
		
		public void CalculateChecksum(byte[] data, int start, int size, ref ChunkChecksum K)
		{
			throw new NotImplementedException();
			// K.v = *reinterpret_cast<CHECKSUM_BLOCK*>(data);
			// K.adler32 = Checksum::adler32(1L,data,size);
		}
	}
}
