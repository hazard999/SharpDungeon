using sharpdungeon.actors;
using System;

namespace sharpdungeon.mechanics
{
	public class Ballistica
	{
		public static int[] Trace = new int[Math.Max(levels.Level.Width, levels.Level.Height)];
		public static int Distance;

		public static int Cast(int from, int to, bool magic, bool hitChars)
		{
			var w = levels.Level.Width;

			var x0 = from % w;
			var x1 = to % w;
			var y0 = from / w;
			var y1 = to / w;

			var dx = x1 - x0;
			var dy = y1 - y0;

			var stepX = dx > 0 ? +1 : -1;
			var stepY = dy > 0 ? +1 : -1;

			dx = Math.Abs(dx);
			dy = Math.Abs(dy);

			int stepA;
			int stepB;
			int dA;
			int dB;

			if (dx > dy)
			{

				stepA = stepX;
				stepB = stepY * w;
				dA = dx;
				dB = dy;

			}
			else
			{

				stepA = stepY * w;
				stepB = stepX;
				dA = dy;
				dB = dx;

			}

			Distance = 1;
			Trace[0] = from;

			var cell = from;

			var err = dA / 2;
			while (cell != to || magic)
			{

				cell += stepA;

				err += dB;
				if (err >= dA)
				{
					err = err - dA;
					cell = cell + stepB;
				}

				Trace[Distance++] = cell;

			    if (!levels.Level.passable[cell] && !levels.Level.avoid[cell])
			        return Trace[--Distance - 1];

			    if (levels.Level.losBlocking[cell] || (hitChars && Actor.FindChar(cell) != null))
			        return cell;
			}

			Trace[Distance++] = cell;

			return to;
		}
	}
}