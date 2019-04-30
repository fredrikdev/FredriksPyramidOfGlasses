using System;

namespace FredriksPyramidOfGlasses {
	public class Glass {
		// time to fill a single glass 
		const double _fillSpeedSec = 10;

		// fields 
		public double _seconds;
		public bool _isTarget, _isRelatedToTarget;
		public Glass _childLeft, _childRight, _parentLeft, _parentRight;

		/// <summary>
		/// Adds the specified seconds to the Glass. Any overflow is directed to the Glasses supporting it.
		/// </summary>
		/// <param name="seconds">The seconds to add to the Glass.</param>
		/// <returns></returns>
		public bool Add(double seconds) {
			var secondsOverflow = (_seconds + seconds) - _fillSpeedSec;
			if (secondsOverflow > 0) {
				// fill seconds is more than we can handle, fill us to the limit
				_seconds = _fillSpeedSec;

				// let the caller know that the target is full
				if (_isTarget)
					return true;

				if ((_childLeft != null && _childRight != null) && (_childLeft._isRelatedToTarget || _childRight._isRelatedToTarget)) {
					secondsOverflow /= 2;
					if (_childLeft._isRelatedToTarget && _childLeft.Add(secondsOverflow))
						return true;
					if (_childRight._isRelatedToTarget && _childRight.Add(secondsOverflow))
						return true;
				}
			} else {
				// fill without overflowing
				_seconds += seconds;
			}

			return false;
		}

		/// <summary>
		/// Creates a jagged array that represents a Pyramid of Glass objects, each Glass object's Left 
		/// and Right properties are connected to the Glasses that supports it. One of the Glasses is to be
		/// marked as the target.
		/// </summary>
		/// <param name="glassRows">The number of rows to create for the Pyramid of Glass objects</param>
		/// <param name="glassRowTarget">The row number (1-based) for the Glass object to mark as the target</param>
		/// <param name="glassColTarget">The column number (1-based) for the Glass object to mark as the target</param>
		/// <returns></returns>
		public static Glass[][] PyramidCreate(int glassRowTarget, int glassColTarget) {
			// create a jagged array that will represent our Pyramid of Glass objects
			var result = new Glass[glassRowTarget][];
			Glass glassTarget = null;
			for (var y = 0; y < glassRowTarget; y++) {
				result[y] = new Glass[y + 1];
				for (var x = 0; x <= y; x++) {
					var isTarget = y + 1 == glassRowTarget && x + 1 == glassColTarget;
					result[y][x] = new Glass() { _isTarget = isTarget };
					if (isTarget)
						glassTarget = result[y][x];
				}
			}

			// each Glass stands on a left and right Glass, and have a left and a right parent Glass
			// (if not last/first row) - anyhow, setup this connection
			for (var y = 0; y < result.Length - 1; y++) {
				for (var x = 0; x < result[y].Length; x++) {
					result[y][x]._childLeft = result[y + 1][x];
					result[y + 1][x]._parentRight = result[y][x];
					result[y][x]._childRight = result[y + 1][x + 1];
					result[y + 1][x + 1]._parentLeft = result[y][x];
				}
			}

			// starting with the taget, we visit each parent to set _isRelatedToTarget on the Glasses that need processing
			bool tagItem(Glass g) {
				g._isRelatedToTarget = true;
				if ((g._parentLeft != null) && (!g._parentLeft._isRelatedToTarget))
					tagItem(g._parentLeft);
				if ((g._parentRight != null) && (!g._parentRight._isRelatedToTarget))
					tagItem(g._parentRight);
				return false;
			}

			tagItem(glassTarget);

			// return the top glass
			return result;
		}

		/// <summary>
		/// Calculates the maximum capacity of a Pyramid of Glass objects
		/// </summary>
		/// <param name="glassRows">The number of rows in the Pyramid</param>
		public static double PyramidMaxCapacity(int glassRows) {
			//2=30,3=70,4=150,5=310,6=630
			double result = 0;
			for (var y = 0; y < glassRows; y++) {
				result = result * 2 + _fillSpeedSec;
			}
			return result;
		}

		/// <summary>
		/// Resets the seconds property of the Pyramid of Glass objects.
		/// </summary>
		/// <param name="glassPyramid">The Pyramid of Glass objects to reset</param>
		public static void PyramidReset(Glass[][] glassPyramid) {
			for (var y = 0; y < glassPyramid.Length; y++)
				for (var x = 0; x < glassPyramid[y].Length; x++)
					glassPyramid[y][x]._seconds = 0;
		}

		/// <summary>
		/// Renders a Pyramid of Glass objects to the Console. The target Glass will be highlighted in green, and
		/// the related/analyzed Glass objects are highlighted in yellow.
		/// </summary>
		/// <param name="glassPyramid">The Pyramid of Glass objects to render</param>
		/// <param name="glassRowTarget">The row number (1-based) for the Glass object to highlight</param>
		/// <param name="glassColTarget">The column number (1-based) for the Glass object to highlight</param>
		public static void PyramidRender(Glass[][] glassPyramid, int glassRowTarget, int glassColTarget) {
			for (var y = 0; y < glassPyramid.Length; y++) {
				Console.Write(new String(' ', (glassPyramid[glassPyramid.Length - 1].Length - y - 1) * 6 / 2));
				for (var x = 0; x < glassPyramid[y].Length; x++) {
					if (y + 1 == glassRowTarget && x + 1 == glassColTarget) {
						Console.ForegroundColor = ConsoleColor.Green;
					} else if (glassPyramid[y][x]._isRelatedToTarget) {
						Console.ForegroundColor = ConsoleColor.Yellow;
					}
					Console.Write(string.Format("{0:00.00} ", glassPyramid[y][x]._seconds));
					Console.ResetColor();
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}
	}
}
