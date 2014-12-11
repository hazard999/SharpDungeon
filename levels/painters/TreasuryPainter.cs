using pdsharp.utils;
using sharpdungeon.items;
using sharpdungeon.items.keys;

namespace sharpdungeon.levels.painters
{
    public class TreasuryPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY);

            Set(level, room.Center(), Terrain.STATUE);

            var heapType = Random.Int(2) == 0 ? Heap.Type.Chest : Heap.Type.Heap;

            var n = Random.IntRange(2, 3);
            for (var i = 0; i < n; i++)
            {
                int pos;
                do
                {
                    pos = room.Random();
                } 
                while (level.map[pos] != Terrain.EMPTY || level.heaps[pos] != null);

                level.Drop(new Gold().Random(), pos).HeapType = heapType;
            }

            if (heapType == Heap.Type.Heap)
            {
                for (var i = 0; i < 6; i++)
                {
                    int pos;
                    do
                    {
                        pos = room.Random();
                    } 
                    while (level.map[pos] != Terrain.EMPTY);

                    level.Drop(new Gold(Random.IntRange(1, 3)), pos);
                }
            }

            room.Entrance().Set(Room.Door.DoorType.LOCKED);
            level.AddItemToSpawn(new IronKey());
        }
    }
}