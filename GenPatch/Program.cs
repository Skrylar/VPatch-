/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/23/2012
 * Time: 3:05 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using VPatch;
using VPatch.Formatter;

namespace GenPatch
{
	class Program : IPatchProgress
	{
		public void OnPatchProgress(long here, long there)
		{
			Console.WriteLine("{0}%", (here/there)*100);
		}
		
		public static void Main(string[] args)
		{
			var app = new Program();
			
			Console.WriteLine("GenPatch# - 2012");
			Console.WriteLine("Ported by Joshua Cearley, based on Koen van de Sande's VPatch.");
			
			Console.WriteLine("Processing...");
			using (var oldF = new FileStream("minecraft2.jar", FileMode.Open))
				using (var newF = new FileStream("minecraft.jar", FileMode.Open))
					using (var patF = new FileStream("patch.dat", FileMode.Create))
			{
				var vp = new VPatch.VPatch();
				vp.CreatePatch(oldF, newF, new PatFormatter(), app, patF);
			}
			Console.WriteLine("Complete.");
			
			Console.ReadLine();
		}
	}
}