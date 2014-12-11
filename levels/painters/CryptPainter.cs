using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.keys;

namespace sharpdungeon.levels.painters
{
    public class CryptPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY);

            var c = room.Center();
            var cx = c.X;
            var cy = c.Y;

            var entrance = room.Entrance();

            entrance.Set(Room.Door.DoorType.LOCKED);
            level.AddItemToSpawn(new IronKey());

            if (entrance.X == room.Left)
            {
                Set(level, new Point(room.Right - 1, room.Top + 1), Terrain.STATUE);
                Set(level, new Point(room.Right - 1, room.Bottom - 1), Terrain.STATUE);
                cx = room.Right - 2;
            }
            else
                if (entrance.X == room.Right)
                {
                    Set(level, new Point(room.Left + 1, room.Top + 1), Terrain.STATUE);
                    Set(level, new Point(room.Left + 1, room.Bottom - 1), Terrain.STATUE);
                    cx = room.Left + 2;
                }
                else
                    if (entrance.Y == room.Top)
                    {
                        Set(level, new Point(room.Left + 1, room.Bottom - 1), Terrain.STATUE);
                        Set(level, new Point(room.Right - 1, room.Bottom - 1), Terrain.STATUE);
                        cy = room.Bottom - 2;
                    }
                    else
                        if (entrance.Y == room.Bottom)
                        {
                            Set(level, new Point(room.Left + 1, room.Top + 1), Terrain.STATUE);
                            Set(level, new Point(room.Right - 1, room.Top + 1), Terrain.STATUE);
                            cy = room.Top + 2;
                        }

            level.Drop(Prize(), cx + cy * Level.Width).HeapType = Heap.Type.Tomb;
        }

        private static Item Prize()
        {
            var prize = Generator.Random(Generator.Category.ARMOR);

            for (var i = 0; i < 3; i++)
            {
                var another = Generator.Random(Generator.Category.ARMOR);
                if (another.level > prize.level)
                    prize = another;
            }

            return prize;
        }
    }
}