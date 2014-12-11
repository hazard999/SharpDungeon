using pdsharp.utils;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;
using sharpdungeon.items.weapon.missiles;

namespace sharpdungeon.levels.painters
{
    public class RatKingPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY_SP);

            var entrance = room.Entrance();
            entrance.Set(Room.Door.DoorType.HIDDEN);
            var door = entrance.X + entrance.Y * Level.Width;

            for (var i = room.Left + 1; i < room.Right; i++)
            {
                AddChest(level, (room.Top + 1) * Level.Width + i, door);
                AddChest(level, (room.Bottom - 1) * Level.Width + i, door);
            }

            for (var i = room.Top + 2; i < room.Bottom - 1; i++)
            {
                AddChest(level, i * Level.Width + room.Left + 1, door);
                AddChest(level, i * Level.Width + room.Right - 1, door);
            }

            var king = new RatKing();
            king.pos = room.Random(1);
            level.mobs.Add(king);
        }

        private static void AddChest(Level level, int pos, int door)
        {
            if (pos == door - 1 || pos == door + 1 || pos == door - Level.Width || pos == door + Level.Width)
                return;

            Item prize;
            switch (Random.Int(10))
            {
                case 0:
                    prize = Generator.Random(Generator.Category.WEAPON);
                    if (prize is MissileWeapon)
                        prize.Quantity(1);
                    else
                        prize.Degrade(Random.Int(3));
                    break;
                case 1:
                    prize = Generator.Random(Generator.Category.ARMOR).Degrade(Random.Int(3));
                    break;
                default:
                    prize = new Gold(Random.IntRange(1, 5));
                    break;
            }

            level.Drop(prize, pos).HeapType = Heap.Type.Chest;
        }
    }
}