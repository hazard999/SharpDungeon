using sharpdungeon.items;
using sharpdungeon.items.potions;

namespace sharpdungeon.levels.painters
{
    public class StoragePainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            const int floor = Terrain.EMPTY_SP;

            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, floor);

            var n = pdsharp.utils.Random.IntRange(3, 4);
            for (var i = 0; i < n; i++)
            {
                int pos;
                do
                {
                    pos = room.Random();
                }
                while (level.map[pos] != floor);
                level.Drop(Prize(level), pos);
            }

            room.Entrance().Set(Room.Door.DoorType.BARRICADE);
            level.AddItemToSpawn(new PotionOfLiquidFlame());
        }

        private static Item Prize(Level level)
        {

            var prize = level.ItemToSpanAsPrize();
            if (prize != null)
                return prize;

            return Generator.Random(pdsharp.utils.Random.OneOf(Generator.Category.POTION, Generator.Category.SCROLL, Generator.Category.FOOD, Generator.Category.GOLD));
        }
    }
}