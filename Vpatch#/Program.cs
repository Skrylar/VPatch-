/*
 * Created by SharpDevelop.
 * User: Skrylar
 * Date: 3/30/2012
 * Time: 5:20 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using VPatch.Interpreter;

namespace VPatch
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("GenPatch# - 2012");
			Console.WriteLine("Ported by Joshua Cearley, based on Koen van de Sande's VPatch.");
			
			Console.WriteLine("Processing");
			using (var oldF = new FileStream("minecraft2.jar", FileMode.Open))
				using (var patF = new FileStream("patch.dat", FileMode.Open))
					using (var newF = new FileStream("shooob.jar", FileMode.Create))
			{
				var vp = new VPatch();
				var result = vp.ApplyPatch(oldF, patF, new PatInterpreter(), null, newF);
				Console.WriteLine(result.ToString());
			}
			
			Console.Write("Complete");
			Console.ReadKey(true);
		}
	}
}