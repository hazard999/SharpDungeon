using System;

//
// * Copyright (C) 2012-2014  Oleg Dolya
// *
// * This program is free software: you can redistribute it and/or modify
// * it under the terms of the GNU General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * This program is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU General Public License for more details.
// *
// * You should have received a copy of the GNU General Public License
// * along with this program.  If not, see <http://www.gnu.org/licenses/>
// 

namespace pdsharp.utils
{

    public class ColorMath
    {

        public static int Interpolate(int A, int B, float p)
        {

            if (p <= 0)
            {
                return A;
            }
            else if (p >= 1)
            {
                return B;
            }

            int ra = A >> 16;
            int ga = (A >> 8) & 0xFF;
            int ba = A & 0xFF;

            int rb = B >> 16;
            int gb = (B >> 8) & 0xFF;
            int bb = B & 0xFF;

            float p1 = 1 - p;

            int r = (int)(p1 * ra + p * rb);
            int g = (int)(p1 * ga + p * gb);
            int b = (int)(p1 * ba + p * bb);

            return (r << 16) + (g << 8) + b;
        }

        public static int Interpolate(float p, params int[] colors)
        {
            if (p <= 0)
                return colors[0];

            if (p >= 1)
                return colors[colors.Length - 1];

            var segment = (int)(colors.Length * p);
            return Interpolate(colors[segment], colors[segment + 1], (p * (colors.Length - 1)) % 1);
        }

        public static int Random(int a, int b)
        {
            return Interpolate(a, b, utils.Random.Float());
        }
    }
}