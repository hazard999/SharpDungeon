using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.plants;

namespace sharpdungeon.levels.painters
{
    public class GardenPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.HIGH_GRASS);
            Fill(level, room, 2, Terrain.GRASS);

            room.Entrance().Set(Room.Door.DoorType.REGULAR);

            var bushes = Random.Int(3) == 0 ? (Random.Int(5) == 0 ? 2 : 1) : 0;
            for (var i = 0; i < bushes; i++)
                level.Plant(new Sungrass.Seed(), room.Random());

            Foliage light;
            if (level.Blobs.ContainsKey(typeof(Foliage)))
                light = (Foliage)level.Blobs[typeof(Foliage)];
            else
                light = new Foliage();

            for (var i = room.Top + 1; i < room.Bottom; i++)
                for (var j = room.Left + 1; j < room.Right; j++)
                    light.Seed(j + Level.Width * i, 1);

            level.Blobs.Add(typeof(Foliage), light);
        }
    }
}