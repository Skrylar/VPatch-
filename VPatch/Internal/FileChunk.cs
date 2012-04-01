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
