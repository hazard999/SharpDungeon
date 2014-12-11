using System;
using sharpdungeon.actors.blobs;

namespace sharpdungeon.levels.painters
{
    public class MagicWellPainter : Painter
    {
        private static readonly Type[] Waters = { typeof(WaterOfAwareness), typeof(WaterOfHealth), typeof(WaterOfTransmutation) };

        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY);

            var c = room.Center();
            Set(level, c.X, c.Y, Terrain.WELL);

            var waterClass = Dungeon.Depth >= Dungeon.Transmutation ? typeof(WaterOfTransmutation) : pdsharp.utils.Random.Element(Waters);

            if (waterClass == typeof(WaterOfTransmutation))
                Dungeon.Transmutation = int.MaxValue;

            if (level.Blobs.ContainsKey(waterClass))
            {
                var water = (WellWater)level.Blobs[waterClass];
                if (water == null)
                {
                    try
                    {
                        water = (WellWater)Activator.CreateInstance(waterClass);
                    }
                    catch (Exception)
                    {
                        water = null;
                    }
                }
                if (water != null)
                {
                    water.Seed(c.X + Level.Width * c.Y, 1);
                    level.Blobs.Add(waterClass, water);
                }
            }

            room.Entrance().Set(Room.Door.DoorType.REGULAR);
        }
    }
}