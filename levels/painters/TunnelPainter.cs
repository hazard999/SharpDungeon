using pdsharp.utils;

namespace sharpdungeon.levels.painters
{
    public class TunnelPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            var floor = level.TunnelTile();

            var c = room.Center();

            if (room.Width() > room.Height() || (room.Width() == room.Height() && Random.Int(2) == 0))
            {
                var from = room.Right - 1;
                var to = room.Left + 1;

                foreach (var door in room.Connected.Values)
                {
                    var step = door.Y < c.Y ? +1 : -1;

                    if (door.X == room.Left)
                    {
                        from = room.Left + 1;
                        for (var i = door.Y; i != c.Y; i += step)
                            Set(level, from, i, floor);
                    }
                    else if (door.X == room.Right)
                    {
                        to = room.Right - 1;
                        for (var i = door.Y; i != c.Y; i += step)
                            Set(level, to, i, floor);
                    }
                    else
                    {
                        if (door.X < from)
                            from = door.X;

                        if (door.X > to)
                            to = door.X;

                        for (var i = door.Y + step; i != c.Y; i += step)
                            Set(level, door.X, i, floor);
                    }
                }

                for (var i = from; i <= to; i++)
                    Set(level, i, c.Y, floor);
            }
            else
            {
                var from = room.Bottom - 1;
                var to = room.Top + 1;

                foreach (var door in room.Connected.Values)
                {
                    var step = door.X < c.X ? +1 : -1;

                    if (door.Y == room.Top)
                    {
                        from = room.Top + 1;
                        for (var i = door.X; i != c.X; i += step)
                            Set(level, i, from, floor);
                    }
                    else if (door.Y == room.Bottom)
                    {
                        to = room.Bottom - 1;
                        for (var i = door.X; i != c.X; i += step)
                            Set(level, i, to, floor);
                    }
                    else
                    {
                        if (door.Y < from)
                            from = door.Y;
                        if (door.Y > to)
                            to = door.Y;

                        for (var i = door.X + step; i != c.X; i += step)
                            Set(level, i, door.Y, floor);
                    }
                }

                for (var i = from; i <= to; i++)
                    Set(level, c.X, i, floor);
            }

            foreach (var door in room.Connected.Values)
                door.Set(Room.Door.DoorType.TUNNEL);
        }
    }
}