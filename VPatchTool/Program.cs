/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/30/2012
 * Time: 8:00 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;
using NConsoler;
using VPatch;
using VPatch.Formatter;

namespace VPatchTool
{
	class Program
	{
		static int retval = 0;
		
		public static int Main(string[] args)
		{
			Console.WriteLine("VPatchTool# - 2012");
			Console.WriteLine("Ported by Joshua Cearley, based on Koen van de Sande's VPatch.\n");
			
			Consolery.Run(typeof(Program), args);
			return retval;
		}
		
		[Action(Description="Creates a differential patch between files.")]
		public static void Create(
			[Required(Description="Outdated file")]
			string oldFileName,
			[Required(Description="Updated file")]
			string newFileName,
			[Required(Description="Filename to save patch under")]
			string patchFileName)
		{
			if (new FileInfo(oldFileName).Exists == false) {
				Console.WriteLine("Error: {0} does not exist!", oldFileName);
				retval = 1;
				return;
			}
			
			if (new FileInfo(newFileName).Exists == false) {
				Console.WriteLine("Error: {0} does not exist!", newFileName);
				retval = 1;
				return;
			}
			
			Console.WriteLine("Creating patch for file {0}...", oldFileName);
			
			var timer = new Stopwatch();
			timer.Start();
			using (var oldF = new FileStream(oldFileName, FileMode.Open))
				using (var newF = new FileStream(newFileName, FileMode.Open))
					using (var patF = new FileStream(patchFileName, FileMode.Create))
			{
				var vp = new VPatch.VPatch();
				vp.CreatePatch(oldF, newF, new PatFormatter(), null, patF);
			}
			timer.Stop();
			Console.WriteLine("Patch created in {0} seconds.", ((float)timer.ElapsedMilliseconds/1000.0f));
		}
		
		[Action(Description="Applies a differential patch to a file.")]
		public static void Apply(
			[Required(Description="Outdated file")]
			string oldFileName,
			[Required(Description="Patch to apply")]
			string patchFileName,
			[Required(Description="Filename to save updated file under")]
			string outputFileName)
		{
			try {
				if (new FileInfo(oldFileName).Exists == false) {
					Console.WriteLine("Error: {0} does not exist!", oldFileName);
					retval = 1;
					return;
				}
				
				if (new FileInfo(patchFileName).Exists == false) {
					Console.WriteLine("Error: {0} does not exist!", patchFileName);
					retval = 1;
					return;
				}
				
				if ((File.GetAttributes(outputFileName) & FileAttributes.Directory) == FileAttributes.Directory) {
					Console.WriteLine("Error: {0} is a directory.", outputFileName);
					retval = 1;
					return;
				}
			} catch (Exception e) {
				Console.WriteLine("Problem checking files: {0}", e.Message);
				retval = 1;
				return;
			}
			
			Console.WriteLine("Updating file {0}...", oldFileName);
			PatchApplyResponse par;
			
			var timer = new Stopwatch();
			timer.Start();
			using (var oldF = new FileStream(oldFileName, FileMode.Open))
				using (var patF = new FileStream(patchFileName, FileMode.Open))
					using (var newF = new FileStream(outputFileName, FileMode.Create))
			{
				var vp = new VPatch.VPatch();
				par = vp.ApplyPatch(oldF, patF, new VPatch.Interpreter.PatInterpreter(), null, newF);
			}
			timer.Stop();
			
			if (par == PatchApplyResponse.Ok) {
				Console.WriteLine("Patch applied in {0} seconds.", ((float)timer.ElapsedMilliseconds/1000.0f));
			} else {
				Console.WriteLine("Patching aborted: {0}", par.ToString());
				new FileInfo(outputFileName).Delete();
			}
		}
	}
}