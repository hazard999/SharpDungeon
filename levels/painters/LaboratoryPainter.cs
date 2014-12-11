using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.items;
using sharpdungeon.items.keys;
using sharpdungeon.items.potions;

namespace sharpdungeon.levels.painters
{
    public class LaboratoryPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY_SP);

            var entrance = room.Entrance();

            Point pot = null;
            if (entrance.X == room.Left)
                pot = new Point(room.Right - 1, Random.Int(2) == 0 ? room.Top + 1 : room.Bottom - 1);
            else if (entrance.X == room.Right)
                pot = new Point(room.Left + 1, Random.Int(2) == 0 ? room.Top + 1 : room.Bottom - 1);
            else if (entrance.Y == room.Top)
                pot = new Point(Random.Int(2) == 0 ? room.Left + 1 : room.Right - 1, room.Bottom - 1);
            else if (entrance.Y == room.Bottom)
                pot = new Point(Random.Int(2) == 0 ? room.Left + 1 : room.Right - 1, room.Top + 1);

            Set(level, pot, Terrain.ALCHEMY);

            var alchemy = new Alchemy();
            alchemy.Seed(pot.X + Level.Width * pot.Y, 1);
            level.Blobs.Add(typeof(Alchemy), alchemy);

            var n = Random.IntRange(2, 3);
            for (var i = 0; i < n; i++)
            {
                int pos;
                do
                {
                    pos = room.Random();
                }
                while (level.map[pos] != Terrain.EMPTY_SP || level.heaps[pos] != null);

                level.Drop(Prize(level), pos);
            }

            entrance.Set(Room.Door.DoorType.LOCKED);
            level.AddItemToSpawn(new IronKey());
        }

        private static Item Prize(Level level)
        {
            var prize = level.ItemToSpanAsPrize();
            if (prize is Potion)
                return prize;

            if (prize != null)
                level.AddItemToSpawn(prize);

            return Generator.Random(Generator.Category.POTION);
        }
    }
}