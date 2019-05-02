/*
	Written by Fredrik Johansson 2019, as a sample program to solve when a certain glass in a Pyramid of Glasses has
	been filled.

	This file is the main entry point of the program. It first prompts the user for how many rows of glasses to build 
	a pyramid of, and then prompts for the index of the glass on the last row to target. Next, it creates a simulation
	of the pyramid, and tries different solutions (by interpolation) until it finds the correct solution with 3 decimals
	of accuaracy.

	The routine is somewhat optimized, especially the inner-loop - Glass().Add(), but not to heavily. Also, when 
	generating the pyramid, I compute if a glass is "related" to the target - i.e. - if it's a glass that may spill 
	over to the target glass.

	Anyhow, I'm not sure this is the best solution, but it sure is one.

	Start with "dotnet run -c Release".
*/
using System;

namespace FredriksPyramidOfGlasses {
	public class Program {
		public static void Main() {
			// init
			Console.WriteLine("Fredrik's Pyramid of Glasses v1.0. Copyright (c) by Fredrik Johansson 2019 (fredrik@johanssonrobotics.com)\n");

			// index (1-based) of the row and column of the glass that we're targeting
			// glassRow also equals to the number of rows in our glass pyramid, first row has 1 glass, second row has 2, and so on
			int glassRow, glassCol;

			// prompt for parameters (just ENTER will exit)
			if (!Prompt($"Enter the number of rows (2 - 50) for the Pyramid, then press ENTER: ", out glassRow, 2, 50))
				return;

			if (!Prompt($"Enter the index of the glass on the last row to target (1 - {glassRow}), then press ENTER: ", out glassCol, 1, glassRow))
				return;

			// compute the answer by testing different solutions on glassPyramid in the range secondsMin-secondsMax
			Console.WriteLine($"Computing the number of seconds it takes to fill glass {glassCol} on the lowest row, in a glass pyramid with {glassRow} rows.");
			var debugTickStartMain = Environment.TickCount;
			double secondsMin = 0, secondsMax = Glass.PyramidMaxCapacity(glassRow);
			double secondsTry = secondsMax;
			var glassPyramid = Glass.PyramidCreate(glassRow, glassCol);
			var glassTop = glassPyramid[0][0];
			while (true) {
				// create a pyramid and test to see if the seconds is to much or to little
				Console.Write($"Answer is between {secondsMin,20:0.000} and {secondsMax,20:0.000}. Testing {secondsTry,20:0.000}... ");

				var debugTickStart = Environment.TickCount;
				Glass.PyramidReset(glassPyramid);
				var addRes = glassTop.Add(secondsTry);
				var debugTickElapsed = Environment.TickCount - debugTickStart;

				double secondsPrev = secondsTry;
				if (addRes) {
					// overflow
					// set the next seconds to try to the mid point between secondsMin and secondsMax 
					Console.WriteLine($"To much (took {debugTickElapsed} msec to compute)");
					secondsMax = secondsTry;
					secondsTry = secondsMin + (secondsMax - secondsMin) / 2;
				} else {
					// set the next seconds to try to the mid point between secondsMin and secondsMax 
					Console.WriteLine($"To little (took {debugTickElapsed} msec to compute)");
					secondsMin = secondsTry;
					secondsTry = secondsMin + (secondsMax - secondsMin) / 2;
				}

				if (Math.Round(secondsTry, 3) == Math.Round(secondsPrev, 3)) {
					// solution found (down to 3 decimals)
					var debugTickElapsedMain = Environment.TickCount - debugTickStartMain;

					Console.WriteLine();
					Glass.PyramidRender(glassPyramid, glassRow, glassCol);
					Console.WriteLine($"Finished! On a glass pyramid with {glassRow} rows, the glass on row {glassRow}, at column {glassCol} is entirely filled after {secondsTry:0.000} seconds (took {debugTickElapsedMain} msec to compute the solution).");
					Console.WriteLine($"NOTE: In the pyramid above, the glass is highlighted in green, and the glasses used to compute this solution are highlighted in yellow.\n");
					Console.WriteLine("Press ENTER to exit...");
					Console.ReadLine();
					break;
				}
			}
		}

		// prompts for user to input an integer in the given range (inclusive). a blank input will return false.
		public static bool Prompt(string prompt, out int result, int min, int max) {
			while (true) {
				Console.Write(prompt);
				var l = Console.ReadLine();
				if (l.Length == 0) {
					result = 0;
					return false;
				}
				if (int.TryParse(l, out result) && result >= min && result <= max)
					return true;
			}
		}
	}
}

