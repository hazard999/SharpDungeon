using System.Collections.Generic;
using System.Linq;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.items;
using sharpdungeon.scenes;
using System;

namespace sharpdungeon.levels
{
    public class SewerBossLevel : RegularLevel
    {
        public SewerBossLevel()
        {
            color1 = 0x48763c;
            color2 = 0x59994a;
        } 

        private int _stairs;

        public override string TilesTex()
        {
            return Assets.TILES_SEWERS;
        }

        public override string WaterTex()
        {
            return Assets.WATER_SEWERS;
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
                distance = RoomEntrance.Distance();

                if (retry++ > 10)
                    return false;
            } 
            while (distance < minDistance);

            RoomEntrance.type = RoomType.ENTRANCE;
            RoomExit.type = RoomType.BOSS_EXIT;

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

            room = RoomExit.Connected.Keys.First();
            if (RoomExit.Top == room.Bottom)
                return false;

            foreach (var r in Rooms.Where(r => r.type == RoomType.NULL && r.Connected.Count > 0))
                r.type = RoomType.TUNNEL;

            var candidates = new List<Room>();
            foreach (var r in RoomExit.Neigbours)
            {
                if (!RoomExit.Connected.ContainsKey(r) && (RoomExit.Left == r.Right || RoomExit.Right == r.Left || RoomExit.Bottom == r.Top))
                    candidates.Add(r);
            }

            if (candidates.Count > 0)
            {
                var kingsRoom = pdsharp.utils.Random.Element(candidates);
                kingsRoom.Connect(RoomExit);
                kingsRoom.type = RoomType.RAT_KING;
            }

            Paint();

            PaintWater();
            PaintGrass();

            PlaceTraps();

            return true;
        }

        protected internal override bool[] Water()
        {
            return Patch.Generate(0.5f, 5);
        }

        protected internal override bool[] Grass()
        {
            return Patch.Generate(0.40f, 4);
        }

        protected internal override void Decorate()
        {
            var start = RoomExit.Top * Width + RoomExit.Left + 1;
            var end = start + RoomExit.Width() - 1;
            for (var i=start; i < end; i++)
            {
                if (i != exit)
                {
                    map[i] = Terrain.WALL_DECO;
                    map[i + Width] = Terrain.WATER;
                }
                else
                    map[i + Width] = Terrain.EMPTY;
            }

            while (true)
            {
                var pos = RoomEntrance.Random();
                
                if (pos == entrance) 
                    continue;

                map[pos] = Terrain.SIGN;
                break;
            }
        }

        public override void AddVisuals(Scene scene)
        {
            SewerLevel.AddVisuals(this, scene);
        }


        protected internal override void CreateMobs()
        {
            var mob = Bestiary.Mob(Dungeon.Depth);
            mob.pos = RoomExit.Random();
            mobs.Add(mob);
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

        public virtual void Seal()
        {
            if (entrance == 0) 
                return;

            Set(entrance, Terrain.WATER_TILES);
            GameScene.UpdateMap(entrance);
            GameScene.Ripple(entrance);

            _stairs = entrance;
            entrance = 0;
        }

        public virtual void Unseal()
        {
            if (_stairs == 0) 
                return;

            entrance = _stairs;
            _stairs = 0;

            Set(entrance, Terrain.ENTRANCE);
            GameScene.UpdateMap(entrance);
        }

        private const string Stairs = "stairs";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(Stairs, _stairs);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            _stairs = bundle.GetInt(Stairs);
        }

        public override string TileName(int tile)
        {
            switch (tile)
            {
            case Terrain.WATER:
                return "Murky water";
            default:
                return base.TileName(tile);
            }
        }

        public override string TileDesc(int tile)
        {
            switch (tile)
            {
            case Terrain.EMPTY_DECO:
                return "Wet yellowish moss covers the floor.";
            default:
                return base.TileDesc(tile);
            }
        }
    }
}