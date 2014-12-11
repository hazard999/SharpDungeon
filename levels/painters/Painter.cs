using Java.Util;
using pdsharp.utils;

namespace sharpdungeon.levels.painters
{
    public abstract class Painter
    {
        public static void Set(Level level, int cell, int value)
        {
            level.map[cell] = value;
        }

        public static void Set(Level level, int X, int Y, int value)
        {
            Set(level, X + Y * Level.Width, value);
        }

        public static void Set(Level level, Point p, int value)
        {
            Set(level, p.X, p.Y, value);
        }

        public static void Fill(Level level, int X, int Y, int w, int h, int value)
        {
            const int width = Level.Width;

            var pos = Y * width + X;
            for (var i = Y; i < Y + h; i++, pos += width)
                for (var j = pos; j < pos + w; j++)
                    level.map[j] = value;
        }

        public static void Fill(Level level, Rect rect, int value)
        {
            Fill(level, rect.Left, rect.Top, rect.Width() + 1, rect.Height() + 1, value);
        }

        public static void Fill(Level level, Rect rect, int m, int value)
        {
            Fill(level, rect.Left + m, rect.Top + m, rect.Width() + 1 - m * 2, rect.Height() + 1 - m * 2, value);
        }

        public static void Fill(Level level, Rect rect, int l, int t, int r, int b, int value)
        {
            Fill(level, rect.Left + l, rect.Top + t, rect.Width() + 1 - (l + r), rect.Height() + 1 - (t + b), value);
        }

        public static Point DrawInside(Level level, Room room, Point from, int n, int value)
        {
            var step = new Point();
            if (from.X == room.Left)
                step.Set(+1, 0);
            else if (from.X == room.Right)
                step.Set(-1, 0);
            else if (from.Y == room.Top)
                step.Set(0, +1);
            else if (from.Y == room.Bottom)
                step.Set(0, -1);

            var p = new Point(from).Offset(step);
            for (var i = 0; i < n; i++)
            {
                if (value != -1)
                    Set(level, p, value);
                p.Offset(step);
            }

            return p;
        }

        public abstract void Paint(Level level, Room room);
    }
}