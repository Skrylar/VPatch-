/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 9:39 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Security.Cryptography;

namespace VPatch.Checksum
{
	public static class MD5
	{
		public static byte[] Check(Stream file)
		{
			var md = new MD5CryptoServiceProvider();
			return md.ComputeHash(file);
		}
	}
}
