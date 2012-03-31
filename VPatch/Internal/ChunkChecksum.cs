/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 5:58 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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
