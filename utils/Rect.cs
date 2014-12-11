using System;

namespace pdsharp.utils
{
    public class Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Rect()
            : this(0, 0, 0, 0)
        {
        }

        public Rect(Rect rect)
            : this(rect.Left, rect.Top, rect.Right, rect.Bottom)
        {
        }

        public Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public virtual int Width()
        {
            return Right - Left;
        }

        public virtual int Height()
        {
            return Bottom - Top;
        }

        public virtual int Square()
        {
            return (Right - Left) * (Bottom - Top);
        }

        public virtual Rect Set(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            return this;
        }

        public virtual Rect Set(Rect rect)
        {
            return Set(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public virtual bool IsEmpty
        {
            get
            {
                return Right <= Left || Bottom <= Top;
            }
        }

        public virtual Rect SetEmpty()
        {
            Left = Right = Top = Bottom = 0;
            return this;
        }

        public virtual Rect Intersect(Rect other)
        {
            var result = new Rect();
            result.Left = Math.Max(Left, other.Left);
            result.Right = Math.Min(Right, other.Right);
            result.Top = Math.Max(Top, other.Top);
            result.Bottom = Math.Min(Bottom, other.Bottom);
            return result;
        }

        public virtual Rect Union(int x, int y)
        {
            if (IsEmpty)
                return Set(x, y, x + 1, y + 1);

            if (x < Left)
                Left = x;
            else
                if (x >= Right)
                    Right = x + 1;

            if (y < Top)
                Top = y;
            else
                if (y >= Bottom)
                    Bottom = y + 1;

            return this;
        }

        public virtual Rect Union(Point p)
        {
            return Union(p.X, p.Y);
        }

        public virtual bool Inside(Point p)
        {
            return p.X >= Left && p.X < Right && p.Y >= Top && p.Y < Bottom;
        }

        public virtual Rect Shrink(int d)
        {
            return new Rect(Left + d, Top + d, Right - d, Bottom - d);
        }

        public virtual Rect Shrink()
        {
            return Shrink(1);
        }
    }
}