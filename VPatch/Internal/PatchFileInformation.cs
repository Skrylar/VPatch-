/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 8:25 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace VPatch.Internal
{
	public class PatchFileInformation
	{
		byte[] mSourceChecksum;
		byte[] mTargetChecksum;
		
		public PatchFileInformation()
		{
		}
		
		public DateTime TargetDateTime { get; set; }
		
		public byte[] SourceChecksum
		{
			get {
				return mSourceChecksum;
			}
			
			set {
				if (value != null && value.Length != 16)
					throw new ArgumentException();
				
				mSourceChecksum = value;
			}
		}
		
		public byte[] TargetChecksum
		{
			get {
				return mTargetChecksum;
			}
			
			set {
				if (value != null && value.Length != 16)
					throw new ArgumentException();
				
				mTargetChecksum = value;
			}
		}
	}
}
