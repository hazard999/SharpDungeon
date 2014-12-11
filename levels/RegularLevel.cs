using System;
using System.Collections.Generic;
using System.Linq;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.items;
using sharpdungeon.items.scrolls;
using sharpdungeon.levels.painters;
using Random = pdsharp.utils.Random;

namespace sharpdungeon.levels
{
    public abstract class RegularLevel : Level
    {
        protected internal List<Room> Rooms;

        protected internal Room RoomEntrance;
        protected internal Room RoomExit;

        protected internal List<RoomType> Specials;

        public int SecretDoors;

        protected override bool Build()
        {
            if (!InitRooms())
                return false;

            int distance;
            var retry = 0;
            var minDistance = (int)Math.Sqrt(Rooms.Count);
            do
            {
                do
                {
                    RoomEntrance = Random.Element(Rooms);
                }
                while (RoomEntrance.Width() < 4 || RoomEntrance.Height() < 4);

                do
                {
                    RoomExit = Random.Element(Rooms);
                }
                while (RoomExit == RoomEntrance || RoomExit.Width() < 4 || RoomExit.Height() < 4);

                Graph.BuildDistanceMap(Rooms, RoomExit);
                distance = RoomEntrance.Distance();

                if (retry++ > 10)
                    return false;
            }
            while (distance < minDistance);

            RoomEntrance.type = RoomType.ENTRANCE;
            RoomExit.type = RoomType.EXIT;

            var connected = new List<Room>();
            connected.Add(RoomEntrance);

            Graph.BuildDistanceMap(Rooms, RoomExit);
            var path = Graph.BuildPath(Rooms, RoomEntrance, RoomExit);

            var room = RoomEntrance;
            foreach (var next in path)
            {
                room.Connect(next);
                room = next;
                connected.Add(room);
            }

            Graph.SetPrice(path, RoomEntrance.distance);

            Graph.BuildDistanceMap(Rooms, RoomExit);
            path = Graph.BuildPath(Rooms, RoomEntrance, RoomExit);

            room = RoomEntrance;
            foreach (var next in path)
            {
                room.Connect(next);
                room = next;
                connected.Add(room);
            }

            var nConnected = (int)(Rooms.Count * Random.Float(0.5f, 0.7f));
            while (connected.Count < nConnected)
            {

                var cr = Random.Element(connected);
                var or = Random.Element(cr.Neigbours);

                if (connected.Contains(or))
                    continue;

                cr.Connect(or);
                connected.Add(or);
            }

            if (Dungeon.ShopOnLevel())
            {
                var shop = RoomEntrance.Connected.Keys.FirstOrDefault(r => r.Connected.Count == 1 && r.Width() >= 5 && r.Height() >= 5);

                if (shop == null)
                    return false;
                shop.type = RoomType.SHOP;
            }

            Specials = new List<RoomType>(levels.Room.SPECIALS);
            if (Dungeon.BossLevel(Dungeon.Depth + 1))
                Specials.Remove(RoomType.WEAK_FLOOR);
            AssignRoomType();

            Paint();
            PaintWater();
            PaintGrass();

            PlaceTraps();

            return true;
        }

        protected internal virtual bool InitRooms()
        {
            Rooms = new List<Room>();
            Split(new Rect(0, 0, Width - 1, Height - 1));

            if (Rooms.Count < 8)
                return false;

            var ra = Rooms;
            for (var i = 0; i < ra.Count - 1; i++)
                for (var j = i + 1; j < ra.Count; j++)
                    ra[i].AddNeigbour(ra[j]);

            return true;
        }

        protected internal virtual void AssignRoomType()
        {
            var specialRooms = 0;

            foreach (var r in Rooms.Where(r => r.type == RoomType.NULL && r.Connected.Count == 1))
            {
                if (Specials.Count > 0 && r.Width() > 3 && r.Height() > 3 && Random.Int(specialRooms * specialRooms + 2) == 0)
                {
                    if (pitRoomNeeded)
                    {
                        r.type = RoomType.PIT;
                        pitRoomNeeded = false;

                        Specials.Remove(RoomType.ARMORY);
                        Specials.Remove(RoomType.CRYPT);
                        Specials.Remove(RoomType.LABORATORY);
                        Specials.Remove(RoomType.LIBRARY);
                        Specials.Remove(RoomType.STATUE);
                        Specials.Remove(RoomType.TREASURY);
                        Specials.Remove(RoomType.VAULT);
                        Specials.Remove(RoomType.WEAK_FLOOR);

                    }
                    else if (Dungeon.Depth % 5 == 2 && Specials.Contains(RoomType.LABORATORY))
                        r.type = RoomType.LABORATORY;
                    else if (Dungeon.Depth >= Dungeon.Transmutation && Specials.Contains(RoomType.MAGIC_WELL))
                        r.type = RoomType.MAGIC_WELL;
                    else
                    {
                        var n = Specials.Count;
                        r.type = Specials[Math.Min(Random.Int(n), Random.Int(n))];

                        if (r.type == RoomType.WEAK_FLOOR)
                            weakFloorCreated = true;
                    }

                    levels.Room.UseType(r.type);
                    Specials.Remove(r.type);
                    specialRooms++;

                }
                else if (Random.Int(2) == 0)
                {
                    var neigbours = r.Neigbours.Where(n => !r.Connected.ContainsKey(n) && !levels.Room.SPECIALS.Contains(n.type) && n.type != RoomType.PIT).ToList();

                    if (neigbours.Count > 1)
                        r.Connect(Random.Element(neigbours));
                }
            }

            var count = 0;
            foreach (var r in Rooms)
            {
                if (r.type != RoomType.NULL)
                    continue;

                var connections = r.Connected.Count;
                switch (connections)
                {
                    case 0:
                        break;
                    default:
                        if (Random.Int(connections * connections) == 0)
                        {
                            r.type = RoomType.STANDARD;
                            count++;
                        }
                        else
                            r.type = RoomType.TUNNEL;

                        break;
                }
            }

            while (count < 4)
            {
                var r = RandomRoom(RoomType.TUNNEL, 1);

                if (r == null)
                    continue;

                r.type = RoomType.STANDARD;
                count++;
            }
        }

        protected internal virtual void PaintWater()
        {
            var lake = Water();
            for (var i = 0; i < Length; i++)
                if (map[i] == Terrain.EMPTY && lake[i])
                    map[i] = Terrain.WATER;
        }

        protected internal virtual void PaintGrass()
        {
            var grass = Grass();

            if (feeling == Feeling.GRASS)
                foreach (var room in Rooms.Where(room => room.type != RoomType.NULL && room.type != RoomType.PASSAGE && room.type != RoomType.TUNNEL))
                {
                    grass[(room.Left + 1) + (room.Top + 1) * Width] = true;
                    grass[(room.Right - 1) + (room.Top + 1) * Width] = true;
                    grass[(room.Left + 1) + (room.Bottom - 1) * Width] = true;
                    grass[(room.Right - 1) + (room.Bottom - 1) * Width] = true;
                }

            for (var i = Width + 1; i < Length - Width - 1; i++)
            {
                if (map[i] != Terrain.EMPTY || !grass[i])
                    continue;

                var count = 1 + NEIGHBOURS8.Count(n => grass[i + n]);
                map[i] = (Random.Float() < count / 12f) ? Terrain.HIGH_GRASS : Terrain.GRASS;
            }
        }

        protected internal abstract bool[] Water();
        protected internal abstract bool[] Grass();

        protected internal virtual void PlaceTraps()
        {
            var numberOfTraps = NumberOfTraps();
            var trapChances = TrapChances();

            for (var i = 0; i < numberOfTraps; i++)
            {
                var trapPos = Random.Int(Length);

                if (map[trapPos] != Terrain.EMPTY)
                    continue;

                switch (Random.Chances(trapChances))
                {
                    case 0:
                        map[trapPos] = Terrain.SECRET_TOXIC_TRAP;
                        break;
                    case 1:
                        map[trapPos] = Terrain.SECRET_FIRE_TRAP;
                        break;
                    case 2:
                        map[trapPos] = Terrain.SECRET_PARALYTIC_TRAP;
                        break;
                    case 3:
                        map[trapPos] = Terrain.SECRET_POISON_TRAP;
                        break;
                    case 4:
                        map[trapPos] = Terrain.SECRET_ALARM_TRAP;
                        break;
                    case 5:
                        map[trapPos] = Terrain.SECRET_LIGHTNING_TRAP;
                        break;
                    case 6:
                        map[trapPos] = Terrain.SECRET_GRIPPING_TRAP;
                        break;
                    case 7:
                        map[trapPos] = Terrain.SECRET_SUMMONING_TRAP;
                        break;
                }
            }
        }

        protected internal virtual int NumberOfTraps()
        {
            return Dungeon.Depth <= 1 ? 0 : Random.Int(1, Rooms.Count + Dungeon.Depth);
        }

        protected internal virtual float[] TrapChances()
        {
            float[] chances = { 1, 1, 1, 1, 1, 1, 1, 1 };
            return chances;
        }

        protected internal int MinRoomSize = 7;
        protected internal int MaxRoomSize = 9;

        protected internal virtual void Split(Rect rect)
        {
            var w = rect.Width();
            var h = rect.Height();

            if (w > MaxRoomSize && h < MinRoomSize)
            {

                var vw = Random.Int(rect.Left + 3, rect.Right - 3);
                Split(new Rect(rect.Left, rect.Top, vw, rect.Bottom));
                Split(new Rect(vw, rect.Top, rect.Right, rect.Bottom));

            }
            else
                if (h > MaxRoomSize && w < MinRoomSize)
                {
                    var vh = Random.Int(rect.Top + 3, rect.Bottom - 3);
                    Split(new Rect(rect.Left, rect.Top, rect.Right, vh));
                    Split(new Rect(rect.Left, vh, rect.Right, rect.Bottom));

                }
                else if ((new System.Random(1).NextDouble() <= (MinRoomSize * MinRoomSize / rect.Square()) && w <= MaxRoomSize && h <= MaxRoomSize) || w < MinRoomSize || h < MinRoomSize)
                    Rooms.Add((Room)new Room().Set(rect));
                else
                {
                    if (Random.Float() < (float)(w - 2) / (w + h - 4))
                    {
                        var vw = Random.Int(rect.Left + 3, rect.Right - 3);
                        Split(new Rect(rect.Left, rect.Top, vw, rect.Bottom));
                        Split(new Rect(vw, rect.Top, rect.Right, rect.Bottom));
                    }
                    else
                    {
                        var vh = Random.Int(rect.Top + 3, rect.Bottom - 3);
                        Split(new Rect(rect.Left, rect.Top, rect.Right, vh));
                        Split(new Rect(rect.Left, vh, rect.Right, rect.Bottom));
                    }
                }
        }

        protected internal virtual void Paint()
        {
            foreach (var r in Rooms)
            {
                if (r.type != RoomType.NULL)
                {
                    PlaceDoors(r);
                    r.Paint(this, r);
                }
                else
                {
                    if (feeling == Feeling.CHASM && Random.Int(2) == 0)
                        Painter.Fill(this, r, Terrain.WALL);
                }
            }

            foreach (var r in Rooms)
                PaintDoors(r);
        }

        private void PlaceDoors(Room r)
        {
            foreach (var n in r.Connected.Keys.ToList())
            {
                var door = r.Connected[n];
                if (door != null)
                    continue;

                var i = r.Intersect(n);
                if (i.Width() == 0)
                    door = new Room.Door(i.Left, Random.Int(i.Top + 1, i.Bottom));
                else
                    door = new Room.Door(Random.Int(i.Left + 1, i.Right), i.Top);

                if (r.Connected.ContainsKey(n))
                    r.Connected[n] = door;
                else
                    r.Connected.Add(n, door);
                n.Connected[r] = door;
            }
        }

        protected internal virtual void PaintDoors(Room r)
        {
            foreach (var n in r.Connected.Keys)
            {
                if (JoinRooms(r, n))
                    continue;

                var d = r.Connected[n];
                var door = d.X + d.Y * Width;

                switch (d.Type)
                {
                    case levels.Room.Door.DoorType.EMPTY:
                        map[door] = Terrain.EMPTY;
                        break;
                    case levels.Room.Door.DoorType.TUNNEL:
                        map[door] = TunnelTile();
                        break;
                    case levels.Room.Door.DoorType.REGULAR:
                        if (Dungeon.Depth <= 1)
                            map[door] = Terrain.DOOR;
                        else
                        {
                            var localSecret = (Dungeon.Depth < 6 ? Random.Int(12 - Dungeon.Depth) : Random.Int(6)) == 0;
                            map[door] = localSecret ? Terrain.SECRET_DOOR : Terrain.DOOR;
                            if (localSecret)
                                SecretDoors++;
                        }
                        break;
                    case levels.Room.Door.DoorType.UNLOCKED:
                        map[door] = Terrain.DOOR;
                        break;
                    case levels.Room.Door.DoorType.HIDDEN:
                        map[door] = Terrain.SECRET_DOOR;
                        break;
                    case levels.Room.Door.DoorType.BARRICADE:
                        map[door] = Random.Int(3) == 0 ? Terrain.BOOKSHELF : Terrain.BARRICADE;
                        break;
                    case levels.Room.Door.DoorType.LOCKED:
                        map[door] = Terrain.LOCKED_DOOR;
                        break;
                }
            }
        }

        protected internal virtual bool JoinRooms(Room r, Room n)
        {
            if (r.type != RoomType.STANDARD || n.type != RoomType.STANDARD)
                return false;

            var w = r.Intersect(n);
            if (w.Left == w.Right)
            {
                if (w.Bottom - w.Top < 3)
                    return false;

                if (w.Height() == Math.Max(r.Height(), n.Height()))
                    return false;

                if (r.Width() + n.Width() > MaxRoomSize)
                    return false;

                w.Top += 1;
                w.Bottom -= 0;

                w.Right++;

                Painter.Fill(this, w.Left, w.Top, 1, w.Height(), Terrain.EMPTY);
            }
            else
            {
                if (w.Right - w.Left < 3)
                    return false;

                if (w.Width() == Math.Max(r.Width(), n.Width()))
                    return false;

                if (r.Height() + n.Height() > MaxRoomSize)
                    return false;

                w.Left += 1;
                w.Right -= 0;

                w.Bottom++;

                Painter.Fill(this, w.Left, w.Top, w.Width(), 1, Terrain.EMPTY);
            }

            return true;
        }

        public override int NMobs()
        {
            return 2 + Dungeon.Depth % 5 + Random.Int(3);
        }

        protected internal override void CreateMobs()
        {
#if DEBUG
            return;
#endif

            var nMobs = NMobs();
            for (var i = 0; i < nMobs; i++)
            {
                var mob = Bestiary.Mob(Dungeon.Depth);

                do
                {
                    mob.pos = RandomRespawnCell();
                }
                while (mob.pos == -1);

                mobs.Add(mob);
                Actor.OccupyCell(mob);
            }
        }

        public override int RandomRespawnCell()
        {
            var count = 0;

            while (true)
            {
                if (++count > 10)
                    return -1;

                var room = RandomRoom(RoomType.STANDARD, 10);
                if (room == null)
                    continue;

                var cell = room.Random();
                if (!Dungeon.Visible[cell] && Actor.FindChar(cell) == null && passable[cell])
                    return cell;
            }
        }

        public override int RandomDestination()
        {
            while (true)
            {
                var room = Random.Element(Rooms);
                if (room == null)
                    continue;

                var cell = room.Random();
                if (passable[cell])
                    return cell;
            }
        }

        protected internal override void CreateItems()
        {
#if DEBUG
            return;
#endif
            var nItems = 3;
            while (Random.Float() < 0.3f)
                nItems++;

            for (var i = 0; i < nItems; i++)
            {
                Heap.Type type;
                switch (Random.Int(20))
                {
                    case 0:
                        type = Heap.Type.Skeleton;
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        type = Heap.Type.Chest;
                        break;
                    default:
                        type = Heap.Type.Heap;
                        break;
                }
                Drop(Generator.Random(), RandomDropCell()).HeapType = type;
            }

            foreach (var itemToSpawn in itemsToSpawn)
            {
                var cell = RandomDropCell();
                if (itemToSpawn is ScrollOfUpgrade)

                    while (map[cell] == Terrain.FIRE_TRAP || map[cell] == Terrain.SECRET_FIRE_TRAP)
                        cell = RandomDropCell();

                Drop(itemToSpawn, cell).HeapType = Heap.Type.Heap;
            }

            var item = Bones.Get();
            if (item != null)
                Drop(item, RandomDropCell()).HeapType = Heap.Type.Skeleton;
        }

        protected internal virtual Room RandomRoom(RoomType type, int tries)
        {
            for (var i = 0; i < tries; i++)
            {
                var room = Random.Element(Rooms);
                if (room.type == type)
                    return room;
            }
            return null;
        }

        public virtual Room Room(int pos)
        {
            return Rooms.FirstOrDefault(room => room.type != RoomType.NULL && room.Inside(pos));
        }

        protected internal virtual int RandomDropCell()
        {
            while (true)
            {
                var room = RandomRoom(RoomType.STANDARD, 1);
                if (room == null)
                    continue;

                var pos = room.Random();
                if (passable[pos])
                    return pos;
            }
        }

        public override int PitCell()
        {
            foreach (var room in Rooms.Where(room => room.type == RoomType.PIT))
                return room.Random();

            return base.PitCell();
        }

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put("rooms", Rooms);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);

            Rooms = bundle.GetCollection("rooms").OfType<Room>().ToList();

            weakFloorCreated = Rooms.Any(r => r.type == RoomType.WEAK_FLOOR);
        }
    }
}