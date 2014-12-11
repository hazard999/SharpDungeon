using System.Linq;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.items;
using sharpdungeon.items.keys;
using sharpdungeon.levels.painters;
using sharpdungeon.scenes;

namespace sharpdungeon.levels
{
    public class PrisonBossLevel : RegularLevel
    {
        public PrisonBossLevel()
        {
            color1 = 0x6a723d;
            color2 = 0x88924c;
        }

        private Room _anteroom;
        private int _arenaDoor;

        private bool _enteredArena;
        private bool _keyDropped;

        public override string TilesTex()
        {
            return Assets.TILES_PRISON;
        }

        public override string WaterTex()
        {
            return Assets.WATER_PRISON;
        }

        private const string Arena = "arena";
        private const string Door = "door";
        private const string Entered = "entered";
        private const string Dropped = "droppped";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(Arena, RoomExit);
            bundle.Put(Door, _arenaDoor);
            bundle.Put(Entered, _enteredArena);
            bundle.Put(Dropped, _keyDropped);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            RoomExit = (Room)bundle.Get(Arena);
            _arenaDoor = bundle.GetInt(Door);
            _enteredArena = bundle.GetBoolean(Entered);
            _keyDropped = bundle.GetBoolean(Dropped);
        }

        protected override bool Build()
        {
            InitRooms();

            int distance;
            var retry = 0;

            do
            {
                if (retry++ > 10)
                    return false;

                var innerRetry = 0;
                do
                {
                    if (innerRetry++ > 10)
                        return false;

                    RoomEntrance = Random.Element(Rooms);
                } while (RoomEntrance.Width() < 4 || RoomEntrance.Height() < 4);

                innerRetry = 0;
                do
                {
                    if (innerRetry++ > 10)
                        return false;

                    RoomExit = Random.Element(Rooms);
                }
                while (RoomExit == RoomEntrance || RoomExit.Width() < 7 || RoomExit.Height() < 7 || RoomExit.Top == 0);

                Graph.BuildDistanceMap(Rooms, RoomExit);
                distance = Graph.BuildPath(Rooms, RoomEntrance, RoomExit).Count;

            } while (distance < 3);

            RoomEntrance.type = RoomType.ENTRANCE;
            RoomExit.type = RoomType.BOSS_EXIT;

            var path = Graph.BuildPath(Rooms, RoomEntrance, RoomExit);
            Graph.SetPrice(path, RoomEntrance.distance);

            Graph.BuildDistanceMap(Rooms, RoomExit);
            path = Graph.BuildPath(Rooms, RoomEntrance, RoomExit);

            _anteroom = path[path.Count - 2];
            _anteroom.type = RoomType.STANDARD;

            var room = RoomEntrance;
            foreach (var next in path)
            {
                room.Connect(next);
                room = next;
            }

            foreach (var room1 in Rooms.Where(room1 => room1.type == RoomType.NULL && room1.Connected.Count > 0))
            {
                room1.type = RoomType.PASSAGE;
            }

            Paint();

            var r = RoomExit.Connected.Keys.ToArray()[0];
            if (RoomExit.Connected[r].Y == RoomExit.Top)
                return false;

            PaintWater();
            PaintGrass();

            PlaceTraps();

            return true;
        }

        protected internal override bool[] Water()
        {
            return Patch.Generate(0.45f, 5);
        }

        protected internal override bool[] Grass()
        {
            return Patch.Generate(0.30f, 4);
        }

        protected internal override void PaintDoors(Room r)
        {
            foreach (var n in r.Connected.Keys)
            {
                if (r.type == RoomType.NULL)
                    continue;

                var door = r.Connected[n];

                if (r.type == RoomType.PASSAGE && n.type == RoomType.PASSAGE)
                    Painter.Set(this, door, Terrain.EMPTY);
                else
                    Painter.Set(this, door, Terrain.DOOR);
            }
        }

        protected internal override void PlaceTraps()
        {
            var traps = NumberOfTraps();

            for (var i = 0; i < traps; i++)
            {
                var trapPos = Random.Int(Length);

                if (map[trapPos] == Terrain.EMPTY)
                    map[trapPos] = Terrain.POISON_TRAP;
            }
        }

        protected internal override void Decorate()
        {
            for (var i = Width + 1; i < Length - Width - 1; i++)
            {
                if (map[i] != Terrain.EMPTY)
                    continue;

                var c = 0.15f;
                if (map[i + 1] == Terrain.WALL && map[i + Width] == Terrain.WALL)
                    c += 0.2f;
                if (map[i - 1] == Terrain.WALL && map[i + Width] == Terrain.WALL)
                    c += 0.2f;
                if (map[i + 1] == Terrain.WALL && map[i - Width] == Terrain.WALL)
                    c += 0.2f;
                if (map[i - 1] == Terrain.WALL && map[i - Width] == Terrain.WALL)
                    c += 0.2f;

                if (Random.Float() < c)
                    map[i] = Terrain.EMPTY_DECO;
            }

            for (var i = 0; i < Width; i++)
                if (map[i] == Terrain.WALL && (map[i + Width] == Terrain.EMPTY || map[i + Width] == Terrain.EMPTY_SP) && Random.Int(4) == 0)
                    map[i] = Terrain.WALL_DECO;

            for (var i = Width; i < Length - Width; i++)
                if (map[i] == Terrain.WALL && map[i - Width] == Terrain.WALL && (map[i + Width] == Terrain.EMPTY || map[i + Width] == Terrain.EMPTY_SP) && Random.Int(2) == 0)
                    map[i] = Terrain.WALL_DECO;

            while (true)
            {
                var pos = RoomEntrance.Random();

                if (pos == entrance)
                    continue;

                map[pos] = Terrain.SIGN;
                break;
            }

            var door = RoomExit.Entrance();
            _arenaDoor = door.X + door.Y * Width;
            Painter.Set(this, _arenaDoor, Terrain.LOCKED_DOOR);

            Painter.Fill(this, RoomExit.Left + 2, RoomExit.Top + 2, RoomExit.Width() - 3, RoomExit.Height() - 3, Terrain.INACTIVE_TRAP);
        }

        protected internal override void CreateMobs()
        {
        }

        public override Actor Respawner()
        {
            return null;
        }

        protected internal override void CreateItems()
        {
            var keyPos = _anteroom.Random();

            while (!passable[keyPos])
            {
                keyPos = _anteroom.Random();
            }

            Drop(new IronKey(), keyPos).HeapType = Heap.Type.Chest;

            var item = Bones.Get();
            if (item == null) return;
            int pos;
            do
            {
                pos = RoomEntrance.Random();
            }
            while (pos == entrance || map[pos] == Terrain.SIGN);

            Drop(item, pos).HeapType = Heap.Type.Skeleton;
        }

        public override void Press(int cell, Character ch)
        {
            base.Press(cell, ch);

            if (ch != Dungeon.Hero || _enteredArena || !RoomExit.Inside(cell))
                return;

            _enteredArena = true;

            int pos;
            do
            {
                pos = RoomExit.Random();
            }
            while (pos == cell || Actor.FindChar(pos) != null);

            var boss = Bestiary.Mob(Dungeon.Depth);
            boss.State = boss.HUNTING;
            boss.pos = pos;
            GameScene.Add(boss);
            boss.Notice();

            MobPress(boss);

            Set(_arenaDoor, Terrain.LOCKED_DOOR);
            GameScene.UpdateMap(_arenaDoor);
            Dungeon.Observe();
        }

        public override Heap Drop(Item item, int cell)
        {
            if (_keyDropped || !(item is SkeletonKey))
                return base.Drop(item, cell);

            _keyDropped = true;

            Set(_arenaDoor, Terrain.DOOR);
            GameScene.UpdateMap(_arenaDoor);
            Dungeon.Observe();

            return base.Drop(item, cell);
        }

        public override int RandomRespawnCell()
        {
            return -1;
        }

        public override string TileName(int tile)
        {
            switch (tile)
            {
                case Terrain.WATER:
                    return "Dark cold water.";
                default:
                    return base.TileName(tile);
            }
        }

        public override string TileDesc(int tile)
        {
            switch (tile)
            {
                case Terrain.EMPTY_DECO:
                    return "There are old blood stains on the floor.";
                default:
                    return base.TileDesc(tile);
            }
        }

        public override void AddVisuals(Scene scene)
        {
            PrisonLevel.AddVisuals(this, scene);
        }
    }
}