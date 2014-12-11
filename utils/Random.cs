using System.Collections.Generic;
using System.Linq;
using Android;

namespace pdsharp.utils
{
    public class Random
    {
        public static float Float(float min, float max)
        {
            return (float)(min + new System.Random().NextDouble() * (max - min));
        }

        public static float Float(float max)
        {
            return (float)(new System.Random().NextDouble() * max);
        }

        public static float Float()
        {
            return (float)new System.Random().NextDouble();
        }

        public static int Int(int max)
        {
            if (max > 0)
            {
                var result = (int)(new System.Random().NextDouble() * max);
                return result;
            }

            return 0;
        }

        public static int Int(int min, int max)
        {
            return min + (int)(new System.Random().NextDouble() * (max - min));
        }

        public static int IntRange(int min, int max)
        {
            return min + (int)(new System.Random().NextDouble() * (max - min + 1));
        }

        public static int NormalIntRange(int min, int max)
        {
            return min + (int)((new System.Random().NextDouble() + new System.Random().NextDouble()) * (max - min + 1) / 2f);
        }

        public static int Chances(float[] chances)
        {
            var sum = chances.Sum();

            var value = sum;
            sum = chances[0];
            for (var i = 0; i < chances.Length; i++)
            {
                if (value <= sum)
                    return i;
                
                if (i == chances.Length - 1)
                    return i;

                sum += chances[i + 1];
            }

            return 0;
        }

        public static TK Chances<TK>(Dictionary<TK, float> chances)
        {
            var size = chances.Count;

            var values = chances.Keys.ToArray();
            var probs = new float[size];
            float sum = 0;
            for (var i = 0; i < size; i++)
            {
                probs[i] = chances[values[i]];
                sum += probs[i];
            }

            var value = sum;

            sum = probs[0];
            for (var i = 0; i < size - 1; i++)
            {
                if (value <= sum)
                    return values[i];

                sum += probs[i + 1];
            }

            if (value <= sum)
                return values[size - 1];

            return default(TK);
        }

        public static int Index<T1>(ICollection<T1> collection)
        {
            return (int)(new System.Random().NextDouble() * collection.Count);
        }

        public static T OneOf<T>(params T[] array)
        {
            return array[(int)(new System.Random().NextDouble() * array.Length)];
        }

        //public static T Element<T>(T[] array)
        //{
        //    return Element(array, array.Length);
        //}

        public static T Element<T>(T[] array, int max)
        {
            return array[(int)(new System.Random().NextDouble() * max)];
        }

        public static T Element<T>(IList<T> collection)
        {
            var size = collection.Count;

            return size > 0 ? collection[Int(size)] : default(T);
        }

        public static void Shuffle<T>(T[] array)
        {
            for (var i = 0; i < array.Length - 1; i++)
            {
                var j = Int(i, array.Length);

                if (j == i)
                    continue;

                var t = array[i];
                array[i] = array[j];
                array[j] = t;
            }
        }

        public static void Shuffle<TU, TV>(TU[] u, TV[] v)
        {
            for (var i = 0; i < u.Length - 1; i++)
            {
                var j = Int(i, u.Length);
                if (j == i)
                    continue;

                var ut = u[i];
                u[i] = u[j];
                u[j] = ut;

                var vt = v[i];
                v[i] = v[j];
                v[j] = vt;
            }
        }
    }
}