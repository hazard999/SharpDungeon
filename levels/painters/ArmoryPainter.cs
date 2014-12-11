using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.keys;

namespace sharpdungeon.levels.painters
{
    public class ArmoryPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY);

            var entrance = room.Entrance();
            Point statue = null;
            if (entrance.X == room.Left)
                statue = new Point(room.Right - 1, Random.Int(2) == 0 ? room.Top + 1 : room.Bottom - 1);
            else if (entrance.X == room.Right)
                statue = new Point(room.Left + 1, Random.Int(2) == 0 ? room.Top + 1 : room.Bottom - 1);
            else if (entrance.Y == room.Top)
                statue = new Point(Random.Int(2) == 0 ? room.Left + 1 : room.Right - 1, room.Bottom - 1);
            else if (entrance.Y == room.Bottom)
                statue = new Point(Random.Int(2) == 0 ? room.Left + 1 : room.Right - 1, room.Top + 1);

            if (statue != null)
                Set(level, statue, Terrain.STATUE);

            var n = Random.IntRange(2, 3);
            for (var i = 0; i < n; i++)
            {
                int pos;
                do
                {
                    pos = room.Random();
                } while (level.map[pos] != Terrain.EMPTY || level.heaps[pos] != null);
                level.Drop(Prize(), pos);
            }

            entrance.Set(Room.Door.DoorType.LOCKED);
            level.AddItemToSpawn(new IronKey());
        }

        private static Item Prize()
        {
            return Generator.Random(Random.OneOf(Generator.Category.ARMOR, Generator.Category.WEAPON));
        }
    }
}