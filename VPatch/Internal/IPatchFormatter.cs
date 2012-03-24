/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 9:43 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace VPatch.Internal
{
	public interface IPatchFormatter
	{
		void FormatPatch(PatchFileInformation fileInfo, IList<SameBlock> sameBlocks, Stream target, Stream output);
	}
}
