/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 5:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace VPatch
{
	/// <summary>
	/// Description of FileChunk.
	/// </summary>
	public struct FileChunk : IComparable<FileChunk>
	{
		public long Offset;
		public ChunkChecksum Checksum;
		
		public int CompareTo(FileChunk other)
		{
			if (Equals(this, other)) return 0;
			if (Checksum < other.Checksum) return -1;
			return 1;
		}
	}
}
