using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.keys;

namespace sharpdungeon.levels.painters
{
    public class PitPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY);

            var entrance = room.Entrance();
            entrance.Set(Room.Door.DoorType.LOCKED);

            Point well = null;
            if (entrance.X == room.Left)
                well = new Point(room.Right - 1, Random.Int(2) == 0 ? room.Top + 1 : room.Bottom - 1);
            else if (entrance.X == room.Right)
                well = new Point(room.Left + 1, Random.Int(2) == 0 ? room.Top + 1 : room.Bottom - 1);
            else if (entrance.Y == room.Top)
                well = new Point(Random.Int(2) == 0 ? room.Left + 1 : room.Right - 1, room.Bottom - 1);
            else if (entrance.Y == room.Bottom)
                well = new Point(Random.Int(2) == 0 ? room.Left + 1 : room.Right - 1, room.Top + 1);

            Set(level, well, Terrain.EMPTY_WELL);

            var remains = room.Random();
            while (level.map[remains] == Terrain.EMPTY_WELL)
                remains = room.Random();

            level.Drop(new IronKey(), remains).HeapType = Heap.Type.Skeleton;

            if (Random.Int(5) == 0)
                level.Drop(Generator.Random(Generator.Category.RING), remains);
            else
                level.Drop(Generator.Random(Random.OneOf(Generator.Category.WEAPON, Generator.Category.ARMOR)), remains);

            var n = Random.IntRange(1, 2);
            for (var i = 0; i < n; i++)
                level.Drop(Prize(level), remains);
        }

        private static Item Prize(Level level)
        {
            var prize = level.ItemToSpanAsPrize();
            if (prize != null)
                return prize;

            return Generator.Random(Random.OneOf(Generator.Category.POTION, Generator.Category.SCROLL, Generator.Category.FOOD, Generator.Category.GOLD));
        }
    }
}