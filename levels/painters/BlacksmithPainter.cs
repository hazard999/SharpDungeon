using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;

namespace sharpdungeon.levels.painters
{
    public class BlacksmithPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.FIRE_TRAP);
            Fill(level, room, 2, Terrain.EMPTY_SP);

            for (var i = 0; i < 2; i++)
            {
                int pos;
                do
                {
                    pos = room.Random();
                }
                while (level.map[pos] != Terrain.EMPTY_SP);
                level.Drop(Generator.Random(Random.OneOf(Generator.Category.ARMOR, Generator.Category.WEAPON)), pos);
            }

            foreach (var door in room.Connected.Values)
            {
                door.Set(Room.Door.DoorType.UNLOCKED);
                DrawInside(level, room, door, 1, Terrain.EMPTY);
            }

            var npc = new Blacksmith();
            do
            {
                npc.pos = room.Random(1);
            }
            while (level.heaps[npc.pos] != null);

            level.mobs.Add(npc);
            Actor.OccupyCell(npc);
        }
    }
}