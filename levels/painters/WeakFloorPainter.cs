using pdsharp.utils;

namespace sharpdungeon.levels.painters
{
    public class WeakFloorPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.CHASM);

            var door = room.Entrance();
            door.Set(Room.Door.DoorType.REGULAR);

            if (door.X == room.Left)
            {
                for (var i = room.Top + 1; i < room.Bottom; i++)
                    DrawInside(level, room, new Point(room.Left, i), Random.IntRange(1, room.Width() - 2), Terrain.EMPTY_SP);
            }
            else
                if (door.X == room.Right)
                {
                    for (var i = room.Top + 1; i < room.Bottom; i++)
                        DrawInside(level, room, new Point(room.Right, i), Random.IntRange(1, room.Width() - 2), Terrain.EMPTY_SP);
                }
                else
                    if (door.Y == room.Top)
                    {
                        for (var i = room.Left + 1; i < room.Right; i++)
                            DrawInside(level, room, new Point(i, room.Top), Random.IntRange(1, room.Height() - 2), Terrain.EMPTY_SP);
                    }
                    else
                        if (door.Y == room.Bottom)
                        {
                            for (var i = room.Left + 1; i < room.Right; i++)
                                DrawInside(level, room, new Point(i, room.Bottom), Random.IntRange(1, room.Height() - 2), Terrain.EMPTY_SP);
                        }
        }
    }
}