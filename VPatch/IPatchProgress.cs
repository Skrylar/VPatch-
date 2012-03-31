/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/27/2012
 * Time: 6:38 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace VPatch
{
	/// <summary>
	/// Description of IPatchProgress.
	/// </summary>
	public interface IPatchProgress
	{
		void OnPatchProgress(long currentPosition, long length);
	}
}
