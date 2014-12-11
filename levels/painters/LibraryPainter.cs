using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.keys;
using sharpdungeon.items.scrolls;

namespace sharpdungeon.levels.painters
{
    public class LibraryPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY);

            var entrance = room.Entrance();
            Point a = null;
            Point b = null;

            if (entrance.X == room.Left)
            {
                a = new Point(room.Left + 1, entrance.Y - 1);
                b = new Point(room.Left + 1, entrance.Y + 1);
                Fill(level, room.Right - 1, room.Top + 1, 1, room.Height() - 1, Terrain.BOOKSHELF);
            }
            else if (entrance.X == room.Right)
            {
                a = new Point(room.Right - 1, entrance.Y - 1);
                b = new Point(room.Right - 1, entrance.Y + 1);
                Fill(level, room.Left + 1, room.Top + 1, 1, room.Height() - 1, Terrain.BOOKSHELF);
            }
            else if (entrance.Y == room.Top)
            {
                a = new Point(entrance.X + 1, room.Top + 1);
                b = new Point(entrance.X - 1, room.Top + 1);
                Fill(level, room.Left + 1, room.Bottom - 1, room.Width() - 1, 1, Terrain.BOOKSHELF);
            }
            else if (entrance.Y == room.Bottom)
            {
                a = new Point(entrance.X + 1, room.Bottom - 1);
                b = new Point(entrance.X - 1, room.Bottom - 1);
                Fill(level, room.Left + 1, room.Top + 1, room.Width() - 1, 1, Terrain.BOOKSHELF);
            }

            if (a != null && level.map[a.X + a.Y*Level.Width] == Terrain.EMPTY)
                Set(level, a, Terrain.STATUE);

            if (b != null && level.map[b.X + b.Y*Level.Width] == Terrain.EMPTY)
                Set(level, b, Terrain.STATUE);

            var n = Random.IntRange(2, 3);
            for (var i = 0; i < n; i++)
            {
                int pos;
                do
                {
                    pos = room.Random();
                } 
                while (level.map[pos] != Terrain.EMPTY || level.heaps[pos] != null);

                level.Drop(Prize(level), pos);
            }

            entrance.Set(Room.Door.DoorType.LOCKED);
            level.AddItemToSpawn(new IronKey());
        }

        private static Item Prize(Level level)
        {
            var prize = level.ItemToSpanAsPrize();
            
            if (prize is Scroll)
                return prize;

            if (prize != null)
                level.AddItemToSpawn(prize);

            return Generator.Random(Generator.Category.SCROLL);
        }
    }
}