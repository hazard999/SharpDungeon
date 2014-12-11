using System;

namespace pdsharp.utils
{
    using FloatMath = Android.Util.FloatMath;

    public class PointF
    {
        public const float Pi = 3.1415926f;
        public const float G2R = Pi / 180;

        public float X;
        public float Y;

        public PointF()
        {
        }

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public PointF(PointF p)
        {
            X = p.X;
            Y = p.Y;
        }

        public PointF(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public virtual PointF Clone()
        {
            return new PointF(this);
        }

        public virtual PointF Scale(float f)
        {
            X *= f;
            Y *= f;
            return this;
        }

        public virtual PointF InvScale(float f)
        {
            X /= f;
            Y /= f;
            return this;
        }

        public virtual PointF Set(float x, float y)
        {
            X = x;
            Y = y;
            return this;
        }

        public virtual PointF Set(PointF p)
        {
            X = p.X;
            Y = p.Y;
            return this;
        }

        public virtual PointF Set(float v)
        {
            X = v;
            Y = v;
            return this;
        }

        public virtual PointF Polar(float a, float l)
        {
            X = l * FloatMath.Cos(a);
            Y = l * FloatMath.Sin(a);
            return this;
        }

        public virtual PointF Offset(float dx, float dy)
        {
            X += dx;
            Y += dy;
            return this;
        }

        public virtual PointF Offset(PointF p)
        {
            X += p.X;
            Y += p.Y;
            return this;
        }

        public virtual PointF Negate()
        {
            X = -X;
            Y = -Y;
            return this;
        }

        public virtual PointF Normalize()
        {
            var l = Length;
            X /= l;
            Y /= l;
            return this;
        }

        public virtual Point Floor()
        {
            return new Point((int)X, (int)Y);
        }

        public virtual float Length
        {
            get { return FloatMath.Sqrt(X * X + Y * Y); }
        }

        public static PointF Sum(PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }

        public static PointF Diff(PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

        public static PointF Inter(PointF a, PointF b, float d)
        {
            return new PointF(a.X + (b.X - a.X) * d, a.Y + (b.Y - a.Y) * d);
        }

        public static float Distance(PointF a, PointF b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return FloatMath.Sqrt(dx * dx + dy * dy);
        }

        public static float Angle(PointF start, PointF end)
        {
            return (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
        }

        public override string ToString()
        {
            return "" + X + ", " + Y;
        }
    }

}