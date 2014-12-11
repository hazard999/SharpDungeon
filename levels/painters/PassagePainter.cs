using System.Collections.Generic;
using Java.Util;
using pdsharp.utils;

namespace sharpdungeon.levels.painters
{
    public class PassagePainter : Painter
    {
        private static int _pasWidth;
        private static int _pasHeight;

        public override void Paint(Level level, Room room)
        {
            _pasWidth = room.Width() - 2;
            _pasHeight = room.Height() - 2;

            var floor = level.TunnelTile();

            var joints = new List<int>();

            foreach (Point door in room.Connected.Values)
                joints.Add(Xy2Point(room, door));

            Collections.Sort(joints);

            var nJoints = joints.Count;
            var perimeter = _pasWidth * 2 + _pasHeight * 2;

            var start = 0;
            var maxD = joints[0] + perimeter - joints[nJoints - 1];
            for (var i = 1; i < nJoints; i++)
            {
                var d = joints[i] - joints[i - 1];

                if (d <= maxD)
                    continue;

                maxD = d;
                start = i;
            }

            var end = (start + nJoints - 1) % nJoints;

            var p = joints[start];
            do
            {
                Set(level, Point2Xy(room, p), floor);
                p = (p + 1) % perimeter;
            }
            while (p != joints[end]);

            Set(level, Point2Xy(room, p), floor);

            foreach (var door in room.Connected.Values)
                door.Set(Room.Door.DoorType.TUNNEL);
        }

        private static int Xy2Point(Room room, Point xy)
        {
            if (xy.Y == room.Top)
                return (xy.Y - room.Left - 1);

            if (xy.Y == room.Right)
                return (xy.Y - room.Top - 1) + _pasWidth;

            if (xy.Y == room.Bottom)
                return (room.Right - xy.X - 1) + _pasWidth + _pasHeight;

            if (xy.Y == room.Top + 1)
                return 0;

            return (room.Bottom - xy.Y - 1) + _pasWidth * 2 + _pasHeight;
        }

        private static Point Point2Xy(Room room, int p)
        {
            if (p < _pasWidth)
                return new Point(room.Left + 1 + p, room.Top + 1);

            if (p < _pasWidth + _pasHeight)
                return new Point(room.Right - 1, room.Top + 1 + (p - _pasWidth));

            if (p < _pasWidth * 2 + _pasHeight)
                return new Point(room.Right - 1 - (p - (_pasWidth + _pasHeight)), room.Bottom - 1);

            return new Point(room.Left + 1, room.Bottom - 1 - (p - (_pasWidth * 2 + _pasHeight)));
        }
    }
}