using System;

namespace pdsharp.glwrap
{
    public class Matrix
    {
        public const float G2RAD = 0.01745329251994329576923690768489f;

        public static float[] Clone(float[] m)
        {
            var n = m.Length;
            var res = new float[n];
            do
            {
                res[--n] = m[n];
            } while (n > 0);

            return res;
        }

        public static void Copy(float[] src, float[] dst)
        {
            var n = src.Length;
            do
            {
                dst[--n] = src[n];
            }
            while (n > 0);
        }

        public static float[] Identity
        {
            set
            {
                for (var i = 0; i < 16; i++)
                    value[i] = 0f;

                for (var i = 0; i < 16; i += 5)
                    value[i] = 1f;
            }
        }

        public static void Rotate(float[] m, float a)
        {
            a *= G2RAD;
            var sin = (float)Math.Sin(a);
            var cos = (float)Math.Cos(a);
            var m0 = m[0];
            var m1 = m[1];
            var m4 = m[4];
            var m5 = m[5];
            m[0] = m0 * cos + m4 * sin;
            m[1] = m1 * cos + m5 * sin;
            m[4] = -m0 * sin + m4 * cos;
            m[5] = -m1 * sin + m5 * cos;
        }

        public static void SkewX(float[] m, float a)
        {
            var t = (float)Math.Tan(a * G2RAD);
            m[4] += -m[0] * t;
            m[5] += -m[1] * t;
        }

        public static void SkewY(float[] m, float a)
        {
            var t = (float)Math.Tan(a * G2RAD);
            m[0] += m[4] * t;
            m[1] += m[5] * t;
        }

        public static void Scale(float[] m, float x, float y)
        {
            m[0] *= x;
            m[1] *= x;
            m[2] *= x;
            m[3] *= x;
            m[4] *= y;
            m[5] *= y;
            m[6] *= y;
            m[7] *= y;
            //	Android.Opengl.Matrix.scaleM( m, 0, x, y, 1 );
        }

        public static void Translate(float[] m, float x, float y)
        {
            m[12] += m[0] * x + m[4] * y;
            m[13] += m[1] * x + m[5] * y;
        }

        public static void Multiply(float[] left, float[] right, float[] result)
        {
            Android.Opengl.Matrix.MultiplyMM(result, 0, left, 0, right, 0);
        }
    }
}