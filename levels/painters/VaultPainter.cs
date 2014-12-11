using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.keys;

namespace sharpdungeon.levels.painters
{
    public class VaultPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {

            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY);

            var cx = (room.Left + room.Right) / 2;
            var cy = (room.Top + room.Bottom) / 2;
            var c = cx + cy * Level.Width;

            switch (Random.Int(3))
            {
                case 0:
                    level.Drop(Prize(), c).HeapType = Heap.Type.LockedChest;
                    level.AddItemToSpawn(new GoldenKey());
                    break;

                case 1:
                    Item i1, i2;
                    do
                    {
                        i1 = Prize();
                        i2 = Prize();
                    } 
                    while (i1.GetType() == i2.GetType());
                    
                    level.Drop(i1, c).HeapType = Heap.Type.CrystalChest;
                    level.Drop(i2, c + Level.NEIGHBOURS8[Random.Int(8)]).HeapType = Heap.Type.CrystalChest;
                    level.AddItemToSpawn(new GoldenKey());
                    
                    break;

                case 2:
                    level.Drop(Prize(), c);
                    Set(level, c, Terrain.PEDESTAL);
                    break;
            }

            room.Entrance().Set(Room.Door.DoorType.LOCKED);
            level.AddItemToSpawn(new IronKey());
        }

        private static Item Prize()
        {
            return Generator.Random(Random.OneOf(Generator.Category.WAND, Generator.Category.RING));
        }
    }
}