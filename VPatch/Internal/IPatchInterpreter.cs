/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/24/2012
 * Time: 9:16 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace VPatch.Internal
{
	public interface IPatchInterpreter
	{
		/// <summary>
		/// Analyzes a patch stream to detect whether it can be intepreted by this
		/// patch interpreter; additionally, load neccesary header data to process
		/// a patch. After analysis the stream will be returned to the original
		/// read position.
		/// </summary>
		/// <param name="patchStream">
		/// Seekable and readable stream to analyze patch data from.
		/// </param>
		/// <returns>
		/// Whether the file was analyzed as a potentially viable patch file, and
		/// the relevant header information has been grabbed.
		/// </returns>
		bool Analyze(Stream patchStream);
		
		/// <summary>
		/// Applies a patch from the patch stream using information gathered
		/// from the <see cref="Analyze" /> step, transforming the oldVersion
		/// in to a new version using the given patchData and writing the
		/// newly updated file to the output stream.
		/// </summary>
		/// <param name="oldVersion">
		/// Seekable stream containing the old version of the file to be updated.
		/// </param>
		/// <param name="patchStream">
		/// Seekable stream containing the patch data, ostensibly the same
		/// stream as used in the analyze step.
		/// </param>
		/// <param name="output">
		/// Stream where the new version of the file will be written to, after
		/// performing any block updates as written in the patch data.
		/// </param>
		/// <param name="prog">
		/// Interface that will receive intermittent progress updates on the
		/// application of the patch. May be null if you don't want this
		/// information.
		/// </param>
		PatchApplyResponse Apply(Stream oldVersion, Stream patchStream, Stream output, IPatchProgress prog);
	}
}
