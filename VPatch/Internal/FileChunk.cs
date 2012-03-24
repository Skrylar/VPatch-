/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 5:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace VPatch.Internal
{
	/// <summary>
	/// Description of FileChunk.
	/// </summary>
	public class FileChunk : IComparable, IComparable<FileChunk>, IEquatable<FileChunk>
	{
		public long Offset;
		public ChunkChecksum Checksum = new ChunkChecksum();
		
		public FileChunk()
		{
		}
		
		public bool Equals(FileChunk other)
		{
			return (Offset == other.Offset) && (Checksum == other.Checksum);
		}
		
		public int CompareTo(object other) {
			if (other is FileChunk) {
				return CompareTo(other as FileChunk);
			} else if (other is ChunkChecksum) {
				return Checksum.CompareTo(other as ChunkChecksum);
			} else {
				throw new ArgumentException();
			}
		}
		
		public int CompareTo(FileChunk other)
		{
			if (Equals(other)) return 0;
			if (Checksum < other.Checksum) return -1;
			return 1;
		}
	}
}
