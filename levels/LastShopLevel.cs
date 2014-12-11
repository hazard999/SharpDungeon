using System;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.actors;
using sharpdungeon.items;

namespace sharpdungeon.levels
{
    public class LastShopLevel : RegularLevel
    {
        public LastShopLevel()
        {
            color1 = 0x4b6636;
            color2 = 0xf2f2f2;
        }

        public override string TilesTex()
        {
            return Assets.TILES_CITY;
        }

        public override string WaterTex()
        {
            return Assets.WATER_CITY;
        }

        protected override bool Build()
        {
            InitRooms();

            int distance;
            var retry = 0;
            var minDistance = (int)Math.Sqrt(Rooms.Count);
            do
            {
                var innerRetry = 0;
                do
                {
                    if (innerRetry++ > 10)
                        return false;

                    RoomEntrance = pdsharp.utils.Random.Element(Rooms);
                }
                while (RoomEntrance.Width() < 4 || RoomEntrance.Height() < 4);

                innerRetry = 0;
                do
                {
                    if (innerRetry++ > 10)
                        return false;

                    RoomExit = pdsharp.utils.Random.Element(Rooms);
                }
                while (RoomExit == RoomEntrance || RoomExit.Width() < 6 || RoomExit.Height() < 6 || RoomExit.Top == 0);

                Graph.BuildDistanceMap(Rooms, RoomExit);
                distance = Graph.BuildPath(Rooms, RoomEntrance, RoomExit).Count;

                if (retry++ > 10)
                    return false;
            }
            while (distance < minDistance);

            RoomEntrance.type = RoomType.ENTRANCE;
            RoomExit.type = RoomType.EXIT;

            Graph.BuildDistanceMap(Rooms, RoomExit);
            var path = Graph.BuildPath(Rooms, RoomEntrance, RoomExit);

            Graph.SetPrice(path, RoomEntrance.distance);

            Graph.BuildDistanceMap(Rooms, RoomExit);
            path = Graph.BuildPath(Rooms, RoomEntrance, RoomExit);

            var room = RoomEntrance;
            foreach (var next in path)
            {
                room.Connect(next);
                room = next;
            }

            Room roomShop = null;
            var shopSquare = 0;
            foreach (var r in Rooms)
            {
                if (r.type != RoomType.NULL || r.Connected.Count <= 0)
                    continue;

                r.type = RoomType.PASSAGE;

                if (r.Square() <= shopSquare)
                    continue;

                roomShop = r;
                shopSquare = r.Square();
            }

            if (roomShop == null || shopSquare < 30)
                return false;

            roomShop.type = Imp.Quest.IsCompleted ? RoomType.SHOP : RoomType.STANDARD;

            Paint();

            PaintWater();
            PaintGrass();

            return true;
        }

        protected internal override void Decorate()
        {
            for (var i = 0; i < Length; i++)
            {
                if (map[i] == Terrain.EMPTY && pdsharp.utils.Random.Int(10) == 0)
                    map[i] = Terrain.EMPTY_DECO;
                else if (map[i] == Terrain.WALL && pdsharp.utils.Random.Int(8) == 0)
                    map[i] = Terrain.WALL_DECO;
                else if (map[i] == Terrain.SECRET_DOOR)
                    map[i] = Terrain.DOOR;
            }

            if (!Imp.Quest.IsCompleted)
                return;

            while (true)
            {
                var pos = RoomEntrance.Random();

                if (pos == entrance)
                    continue;

                map[pos] = Terrain.SIGN;
                break;
            }
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
            var item = Bones.Get();

            if (item == null)
                return;

            int pos;
            do
            {
                pos = RoomEntrance.Random();
            }
            while (pos == entrance || map[pos] == Terrain.SIGN);

            Drop(item, pos).HeapType = Heap.Type.Skeleton;
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
                    return "Suspiciously colored water";
                case Terrain.HIGH_GRASS:
                    return "High blooming flowers";
                default:
                    return base.TileName(tile);
            }
        }

        public override string TileDesc(int tile)
        {
            switch (tile)
            {
                case Terrain.ENTRANCE:
                    return "A ramp leads up to the upper depth.";
                case Terrain.EXIT:
                    return "A ramp leads down to the Inferno.";
                case Terrain.WALL_DECO:
                case Terrain.EMPTY_DECO:
                    return "Several tiles are missing here.";
                case Terrain.EMPTY_SP:
                    return "Thick carpet covers the floor.";
                default:
                    return base.TileDesc(tile);
            }
        }

        protected internal override bool[] Water()
        {
            return Patch.Generate(0.35f, 4);
        }

        protected internal override bool[] Grass()
        {
            return Patch.Generate(0.30f, 3);
        }

        public override void AddVisuals(Scene scene)
        {
            CityLevel.AddVisuals(this, scene);
        }
    }
}