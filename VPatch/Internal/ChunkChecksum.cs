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
	public struct ChunkChecksum : IComparable<ChunkChecksum>
	{
		public ulong Adler32;
		public Int64 V;
		
		public int CompareTo(ChunkChecksum other)
		{
			// Equality check.
			if ((V == other.V) &&
			    (Adler32 == other.Adler32))
			{
				return 0;
			}
			
			if (Adler32 < other.Adler32) return -1;
			if (Adler32 == other.Adler32) {
				if (V < other.V) {
					return -1;
				} else {
					return 1;
				}
			} else {
				return 1;
			}
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is ChunkChecksum) && Equals((ChunkChecksum)obj);
		}
		
		public bool Equals(ChunkChecksum other)
		{
			return this.Adler32 == other.Adler32 && this.V == other.V;
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
		
		public static bool operator ==(ChunkChecksum lhs, ChunkChecksum rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(ChunkChecksum lhs, ChunkChecksum rhs)
		{
			return !(lhs == rhs);
		}
		
		public static bool operator <(ChunkChecksum lhs, ChunkChecksum rhs)
		{
			return (lhs.CompareTo(rhs) == -1);
		}
		
		public static bool operator >(ChunkChecksum lhs, ChunkChecksum rhs)
		{
			return (lhs.CompareTo(rhs) == 1);
		}
		#endregion

	}
}
