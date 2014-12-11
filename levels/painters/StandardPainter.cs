using System;
using pdsharp.utils;
using sharpdungeon.items;
using Random = pdsharp.utils.Random;

namespace sharpdungeon.levels.painters
{
    public class StandardPainter : Painter
    {
        public override void Paint(Level level, Room room)
        {
            Fill(level, room, Terrain.WALL);
            foreach (var door in room.Connected.Values)
                door.Set(Room.Door.DoorType.REGULAR);

            if (!Dungeon.BossLevel() && Random.Int(5) == 0)
            {
                switch (Random.Int(6))
                {
                    case 0:
                        if (level.feeling != Level.Feeling.GRASS)
                            if (Math.Min(room.Width(), room.Height()) >= 4 && Math.Max(room.Width(), room.Height()) >= 6)
                            {
                                PaintGraveyard(level, room);
                                return;
                            }

                        break;
                    case 1:
                        if (Dungeon.Depth > 1)
                        {
                            PaintBurned(level, room);
                            return;
                        }
                        break;
                    case 2:
                        if (Math.Max(room.Width(), room.Height()) >= 4)
                        {
                            PaintStriped(level, room);
                            return;
                        }
                        break;
                    case 3:
                        if (room.Width() >= 6 && room.Height() >= 6)
                        {
                            PaintStudy(level, room);
                            return;
                        }
                        break;
                    case 4:
                        if (level.feeling != Level.Feeling.WATER)
                        {
                            if (room.Connected.Count == 2 && room.Width() >= 4 && room.Height() >= 4)
                            {
                                PaintBridge(level, room);
                                return;
                            }
                        }
                        break;
                    case 5:
                        if (!Dungeon.BossLevel() && !Dungeon.BossLevel(Dungeon.Depth + 1) && Math.Min(room.Width(), room.Height()) >= 5)
                        {
                            PaintFissure(level, room);
                            return;
                        }
                        break;
                }
            }
            
            Fill(level, room, 1, Terrain.EMPTY);
        }

        private static void PaintBurned(Level level, Room room)
        {
            for (var i = room.Top + 1; i < room.Bottom; i++)
                for (var j = room.Left + 1; j < room.Right; j++)
                {
                    var t = Terrain.EMBERS;
                    switch (Random.Int(5))
                    {
                        case 0:
                            t = Terrain.EMPTY;
                            break;
                        case 1:
                            t = Terrain.FIRE_TRAP;
                            break;
                        case 2:
                            t = Terrain.SECRET_FIRE_TRAP;
                            break;
                        case 3:
                            t = Terrain.INACTIVE_TRAP;
                            break;
                    }

                    level.map[i * Level.Width + j] = t;
                }
        }

        private static void PaintGraveyard(Level level, Room room)
        {
            Fill(level, room.Left + 1, room.Top + 1, room.Width() - 1, room.Height() - 1, Terrain.GRASS);

            var w = room.Width() - 1;
            var h = room.Height() - 1;
            var nGraves = Math.Max(w, h) / 2;

            var index = Random.Int(nGraves);

            var shift = Random.Int(2);
            for (var i = 0; i < nGraves; i++)
            {
                var pos = w > h ? room.Left + 1 + shift + i * 2 + (room.Top + 2 + Random.Int(h - 2)) * Level.Width : (room.Left + 2 + Random.Int(w - 2)) + (room.Top + 1 + shift + i * 2) * Level.Width;
                level.Drop(i == index ? Generator.Random() : new Gold(), pos).HeapType = Heap.Type.Tomb;
            }
        }

        private static void PaintStriped(Level level, Room room)
        {
            Fill(level, room.Left + 1, room.Top + 1, room.Width() - 1, room.Height() - 1, Terrain.EMPTY_SP);

            if (room.Width() > room.Height())
            {
                for (var i = room.Left + 2; i < room.Right; i += 2)
                    Fill(level, i, room.Top + 1, 1, room.Height() - 1, Terrain.HIGH_GRASS);
            }
            else
            {
                for (var i = room.Top + 2; i < room.Bottom; i += 2)
                    Fill(level, room.Left + 1, i, room.Width() - 1, 1, Terrain.HIGH_GRASS);
            }
        }

        private static void PaintStudy(Level level, Room room)
        {
            Fill(level, room.Left + 1, room.Top + 1, room.Width() - 1, room.Height() - 1, Terrain.BOOKSHELF);
            Fill(level, room.Left + 2, room.Top + 2, room.Width() - 3, room.Height() - 3, Terrain.EMPTY_SP);

            foreach (var door in room.Connected.Values)
            {
                if (door.X == room.Left)
                    Set(level, door.X + 1, door.Y, Terrain.EMPTY);
                else
                    if (door.X == room.Right)
                        Set(level, door.X - 1, door.Y, Terrain.EMPTY);
                    else
                        if (door.Y == room.Top)
                            Set(level, door.X, door.Y + 1, Terrain.EMPTY);
                        else
                            if (door.Y == room.Bottom)
                                Set(level, door.X, door.Y - 1, Terrain.EMPTY);
            }

            Set(level, room.Center(), Terrain.PEDESTAL);
        }

        private static void PaintBridge(Level level, Room room)
        {
            Fill(level, room.Left + 1, room.Top + 1, room.Width() - 1, room.Height() - 1, !Dungeon.BossLevel() && !Dungeon.BossLevel(Dungeon.Depth + 1) && Random.Int(3) == 0 ? Terrain.CHASM : Terrain.WATER);

            Point door1 = null;
            Point door2 = null;
            foreach (var p in room.Connected.Values)
            {
                if (door1 == null)
                    door1 = p;
                else
                    door2 = p;
            }

            if ((door1.X == room.Left && door2.X == room.Right) || (door1.X == room.Right && door2.X == room.Left))
            {
                var s = room.Width() / 2;

                DrawInside(level, room, door1, s, Terrain.EMPTY_SP);
                DrawInside(level, room, door2, s, Terrain.EMPTY_SP);
                Fill(level, room.Center().X, Math.Min(door1.Y, door2.Y), 1, Math.Abs(door1.Y - door2.Y) + 1, Terrain.EMPTY_SP);

            }
            else
                if ((door1.Y == room.Top && door2.Y == room.Bottom) || (door1.Y == room.Bottom && door2.Y == room.Top))
                {

                    int s = room.Height() / 2;

                    DrawInside(level, room, door1, s, Terrain.EMPTY_SP);
                    DrawInside(level, room, door2, s, Terrain.EMPTY_SP);
                    Fill(level, Math.Min(door1.X, door2.X), room.Center().Y, Math.Abs(door1.X - door2.X) + 1, 1, Terrain.EMPTY_SP);

                }
                else
                    if (door1.X == door2.X)
                        Fill(level, door1.X == room.Left ? room.Left + 1 : room.Right - 1, Math.Min(door1.Y, door2.Y), 1, Math.Abs(door1.Y - door2.Y) + 1, Terrain.EMPTY_SP);
                    else
                        if (door1.Y == door2.Y)
                            Fill(level, Math.Min(door1.X, door2.X), door1.Y == room.Top ? room.Top + 1 : room.Bottom - 1, Math.Abs(door1.X - door2.X) + 1, 1, Terrain.EMPTY_SP);
                        else
                            if (door1.Y == room.Top || door1.Y == room.Bottom)
                            {
                                DrawInside(level, room, door1, Math.Abs(door1.Y - door2.Y), Terrain.EMPTY_SP);
                                DrawInside(level, room, door2, Math.Abs(door1.X - door2.X), Terrain.EMPTY_SP);
                            }
                            else
                                if (door1.X == room.Left || door1.X == room.Right)
                                {
                                    DrawInside(level, room, door1, Math.Abs(door1.X - door2.X), Terrain.EMPTY_SP);
                                    DrawInside(level, room, door2, Math.Abs(door1.Y - door2.Y), Terrain.EMPTY_SP);
                                }
        }

        private static void PaintFissure(Level level, Room room)
        {
            Fill(level, room.Left + 1, room.Top + 1, room.Width() - 1, room.Height() - 1, Terrain.EMPTY);

            for (var i = room.Top + 2; i < room.Bottom - 1; i++)
                for (var j = room.Left + 2; j < room.Right - 1; j++)
                {
                    var v = Math.Min(i - room.Top, room.Bottom - i);
                    var h = Math.Min(j - room.Left, room.Right - j);
                    
                    if (Math.Min(v, h) > 2 || Random.Int(2) == 0)
                        Set(level, j, i, Terrain.CHASM);
                }
        }
    }
}