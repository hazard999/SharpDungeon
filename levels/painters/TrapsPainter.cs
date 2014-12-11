using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.potions;

namespace sharpdungeon.levels.painters
{
    public class TrapsPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            int[] traps = { Terrain.TOXIC_TRAP, Terrain.TOXIC_TRAP, Terrain.TOXIC_TRAP, Terrain.PARALYTIC_TRAP, Terrain.PARALYTIC_TRAP, !Dungeon.BossLevel(Dungeon.Depth + 1) ? Terrain.CHASM : Terrain.SUMMONING_TRAP };

            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, pdsharp.utils.Random.Element(traps));

            var door = room.Entrance();
            door.Set(Room.Door.DoorType.REGULAR);

            var lastRow = level.map[room.Left + 1 + (room.Top + 1) * Level.Width] == Terrain.CHASM ? Terrain.CHASM : Terrain.EMPTY;

            var x = -1;
            var y = -1;
            if (door.X == room.Left)
            {
                x = room.Right - 1;
                y = room.Top + room.Height() / 2;
                Fill(level, x, room.Top + 1, 1, room.Height() - 1, lastRow);
            }
            else if (door.X == room.Right)
            {
                x = room.Left + 1;
                y = room.Top + room.Height() / 2;
                Fill(level, x, room.Top + 1, 1, room.Height() - 1, lastRow);
            }
            else if (door.Y == room.Top)
            {
                x = room.Left + room.Width() / 2;
                y = room.Bottom - 1;
                Fill(level, room.Left + 1, y, room.Width() - 1, 1, lastRow);
            }
            else if (door.Y == room.Bottom)
            {
                x = room.Left + room.Width() / 2;
                y = room.Top + 1;
                Fill(level, room.Left + 1, y, room.Width() - 1, 1, lastRow);
            }

            var pos = x + y * Level.Width;
            if (Random.Int(3) == 0)
            {
                if (lastRow == Terrain.CHASM)
                    Set(level, pos, Terrain.EMPTY);
                level.Drop(Prize(level), pos).HeapType = Heap.Type.Chest;
            }
            else
            {
                Set(level, pos, Terrain.PEDESTAL);
                level.Drop(Prize(level), pos);
            }

            level.AddItemToSpawn(new PotionOfLevitation());
        }

        private static Item Prize(Level level)
        {
            var prize = level.ItemToSpanAsPrize();
            if (prize != null)
                return prize;

            prize = Generator.Random(Random.OneOf(Generator.Category.WEAPON, Generator.Category.ARMOR));

            for (var i = 0; i < 3; i++)
            {
                var another = Generator.Random(Random.OneOf(Generator.Category.WEAPON, Generator.Category.ARMOR));
                if (another.level > prize.level)
                    prize = another;
            }

            return prize;
        }
    }
}