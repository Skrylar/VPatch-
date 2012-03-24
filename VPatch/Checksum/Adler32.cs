/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 9:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace VPatch.Checksum
{
	public static class Adler32
	{
		public const int BASE = 65521;
		public const int NMAX = 5552;
		
		public static ulong Check(ulong adler, byte[] buf)
		{
			unchecked {
				ulong s1 = adler & 0xFFFF;
				ulong s2 = (adler >> 16) & 0xFFFF;
				int k;
				
				if (buf == null) return 1;
				if (buf.Length == 0) return 1;
				
				long len = buf.LongLength;
				long idx = 0;
				
				while (len > 0) {
					k = (len < NMAX) ? (int)len : NMAX;
					len -= k;
					while (k >= 16) {
						for (int j = 0; j < 16; j++) {
							s1 += buf[idx+j];
							s2 += s1;
						}
						idx += 16;
						k -= 16;
					}
					if (k != 0) {
						do {
							s1 += buf[idx++];
							s2 += s1;
						} while (--k != 0);
					}
					s1 %= BASE;
					s2 %= BASE;
				}
				
				return (s2 << 16) | s1;
			}
		}
	}
}
