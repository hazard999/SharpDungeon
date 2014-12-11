using System;
using pdsharp.noosa;

namespace pdsharp.utils
{
    public class GameMath
    {
        public static float Speed(float speed, float acc)
        {
            if (Math.Abs(acc) > 0.0001)
                speed += acc * Game.Elapsed;

            return speed;
        }

        public static float Gate(float min, float value, float max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }
    }

}