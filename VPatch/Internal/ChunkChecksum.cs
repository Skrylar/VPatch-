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
	public class ChunkChecksum : IEquatable<ChunkChecksum>, IComparable, IComparable<ChunkChecksum>
	{
		public ulong Adler32;
		public UInt64 V;
		
		public override string ToString()
		{
			return string.Format("[ChunkChecksum Adler32={0}, V={1}]", Adler32, V);
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * Adler32.GetHashCode();
				hashCode += 1000000009 * V.GetHashCode();
			}
			return hashCode;
		}
		
		public int CompareTo(object other)
		{
			if (other is ChunkChecksum) {
				return CompareTo(other as ChunkChecksum);
			} else {
				throw new ArgumentException();
			}
		}
		
		public int CompareTo(ChunkChecksum other)
		{
			if (Equals(other)) return 0;
			if (Adler32 < other.Adler32) return -1;
			if (Adler32 == other.Adler32) {
				if (V < other.V) return -1;
			}
			return 1;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is ChunkChecksum) {
				return Equals(obj as ChunkChecksum);
			} else {
				throw new ArgumentException();
			}
		}
		
		public bool Equals(ChunkChecksum other)
		{
			if (Adler32 == other.Adler32 && V == other.V)
				return true;
			return false;
		}

		public static bool operator ==(ChunkChecksum lhs, ChunkChecksum rhs)
		{
			return (lhs.Equals(rhs));
		}
		
		public static bool operator !=(ChunkChecksum lhs, ChunkChecksum rhs)
		{
			return !(lhs == rhs);
		}
		
		public static bool operator <(ChunkChecksum l, ChunkChecksum r)
		{
			return (l.CompareTo(r) == -1);
		}
		
		public static bool operator >(ChunkChecksum l, ChunkChecksum r)
		{
			return (l.CompareTo(r) == 1);
		}
	}
}
