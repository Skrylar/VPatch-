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

namespace VPatch.Checksum
{
	public static class Adler32
	{
		public const int BASE = 65521;
		public const int NMAX = 5552;
		
		public static ulong Check(ulong adler, byte[] buf, long start, long size)
		{
			//Console.WriteLine("Adlering {0} bytes from {1}.", size, start);
			unchecked {
				ulong s1 = adler & 0xFFFF;
				ulong s2 = (adler >> 16) & 0xFFFF;
				int k;
				
				if (buf == null) return 1;
				if (buf.Length == 0) return 1;
				
				long len = size;
				long idx = start;
				
				while (len > 0) {
					k = (len < NMAX) ? (int)len : NMAX;
					len -= k;
					while (k > 0) {
						s1 += buf[idx++];
						s2 += s1;
						k--;
					}
					s1 %= BASE;
					s2 %= BASE;
				}
				
				return (s2 << 16) | s1;
			}
		}
	}
}
