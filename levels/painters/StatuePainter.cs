using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.items.keys;

namespace sharpdungeon.levels.painters
{
    public class StatuePainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            Fill(level, room, 1, Terrain.EMPTY);

            var c = room.Center();
            var cx = c.X;
            var cy = c.Y;

            var door = room.Entrance();
            door.Set(Room.Door.DoorType.LOCKED);

            level.AddItemToSpawn(new IronKey());

            if (door.X == room.Left)
            {
                Fill(level, room.Right - 1, room.Top + 1, 1, room.Height() - 1, Terrain.STATUE);
                cx = room.Right - 2;
            }
            else
                if (door.X == room.Right)
                {
                    Fill(level, room.Left + 1, room.Top + 1, 1, room.Height() - 1, Terrain.STATUE);
                    cx = room.Left + 2;
                }
                else
                    if (door.Y == room.Top)
                    {
                        Fill(level, room.Left + 1, room.Bottom - 1, room.Width() - 1, 1, Terrain.STATUE);
                        cy = room.Bottom - 2;
                    }
                    else
                        if (door.Y == room.Bottom)
                        {
                            Fill(level, room.Left + 1, room.Top + 1, room.Width() - 1, 1, Terrain.STATUE);
                            cy = room.Top + 2;
                        }

            var statue = new Statue();
            statue.pos = cx + cy * Level.Width;
            level.mobs.Add(statue);
            Actor.OccupyCell(statue);
        }
    }
}