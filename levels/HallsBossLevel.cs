using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.items.keys;
using sharpdungeon.levels.painters;
using sharpdungeon.scenes;

namespace sharpdungeon.levels
{
    public class HallsBossLevel : Level
    {
        public HallsBossLevel()
        {
            color1 = 0x801500;
            color2 = 0xa68521;

            viewDistance = 3;
        }

        private const int RoomLeft = Width / 2 - 1;
        private const int RoomRight = Width / 2 + 1;
        private const int RoomTop = Height / 2 - 1;
        private const int RoomBottom = Height / 2 + 1;

        private int _stairs = -1;
        private bool _enteredArena;
        private bool _keyDropped;

        public override string TilesTex()
        {
            return Assets.TILES_HALLS;
        }

        public override string WaterTex()
        {
            return Assets.WATER_HALLS;
        }

        private const string STAIRS = "stairs";
        private const string ENTERED = "entered";
        private const string DROPPED = "droppped";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(STAIRS, _stairs);
            bundle.Put(ENTERED, _enteredArena);
            bundle.Put(DROPPED, _keyDropped);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            _stairs = bundle.GetInt(STAIRS);
            _enteredArena = bundle.GetBoolean(ENTERED);
            _keyDropped = bundle.GetBoolean(DROPPED);
        }

        protected override bool Build()
        {
            for (var i = 0; i < 5; i++)
            {
                var top = Random.IntRange(2, RoomTop - 1);
                var bottom = Random.IntRange(RoomBottom + 1, 22);
                Painter.Fill(this, 2 + i * 4, top, 4, bottom - top + 1, Terrain.EMPTY);

                if (i == 2)
                    exit = (i * 4 + 3) + (top - 1) * Width;

                for (var j = 0; j < 4; j++)
                {
                    if (Random.Int(2) != 0)
                        continue;

                    var y = Random.IntRange(top + 1, bottom - 1);
                    map[i * 4 + j + y * Width] = Terrain.WALL_DECO;
                }
            }

            map[exit] = Terrain.LOCKED_EXIT;

            Painter.Fill(this, RoomLeft - 1, RoomTop - 1, RoomRight - RoomLeft + 3, RoomBottom - RoomTop + 3, Terrain.WALL);
            Painter.Fill(this, RoomLeft, RoomTop, RoomRight - RoomLeft + 1, RoomBottom - RoomTop + 1, Terrain.EMPTY);

            entrance = Random.Int(RoomLeft + 1, RoomRight - 1) + Random.Int(RoomTop + 1, RoomBottom - 1) * Width;
            map[entrance] = Terrain.ENTRANCE;

            var patch = Patch.Generate(0.45f, 6);
            for (var i = 0; i < Length; i++)
                if (map[i] == Terrain.EMPTY && patch[i])
                    map[i] = Terrain.WATER;

            return true;
        }

        protected internal override void Decorate()
        {
            for (var i = 0; i < Length; i++)
                if (map[i] == Terrain.EMPTY && Random.Int(10) == 0)
                    map[i] = Terrain.EMPTY_DECO;
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
            Item item = Bones.Get();
            
            if (item == null) 
                return;

            int pos;
            do
            {
                pos = Random.IntRange(RoomLeft, RoomRight) + Random.IntRange(RoomTop + 1, RoomBottom) * Width;
            } 
            while (pos == entrance || map[pos] == Terrain.SIGN);

            Drop(item, pos).HeapType = Heap.Type.Skeleton;
        }

        public override int RandomRespawnCell()
        {
            return -1;
        }

        public override void Press(int cell, Character hero)
        {
            base.Press(cell, hero);

            if (_enteredArena || hero != Dungeon.Hero || cell == entrance) 
                return;

            _enteredArena = true;

            for (var i = RoomLeft - 1; i <= RoomRight + 1; i++)
            {
                DoMagic((RoomTop - 1) * Width + i);
                DoMagic((RoomBottom + 1) * Width + i);
            }

            for (var i = RoomTop; i < RoomBottom + 1; i++)
            {
                DoMagic(i * Width + RoomLeft - 1);
                DoMagic(i * Width + RoomRight + 1);
            }

            DoMagic(entrance);
            GameScene.UpdateMap();

            Dungeon.Observe();

            var boss = new Yog();
            do
            {
                boss.pos = Random.Int(Length);
            } 
            while (!passable[boss.pos] || Dungeon.Visible[boss.pos]);

            GameScene.Add(boss);
            boss.SpawnFists();

            _stairs = entrance;
            entrance = -1;
        }

        private void DoMagic(int cell)
        {
            Set(cell, Terrain.EMPTY_SP);
            CellEmitter.Get(cell).Start(FlameParticle.Factory, 0.1f, 3);
        }

        public override Heap Drop(Item item, int cell)
        {
            if (_keyDropped || !(item is SkeletonKey)) 
                return base.Drop(item, cell);

            _keyDropped = true;

            entrance = _stairs;
            Set(entrance, Terrain.ENTRANCE);
            GameScene.UpdateMap(entrance);

            return base.Drop(item, cell);
        }

        public override string TileName(int tile)
        {
            switch (tile)
            {
                case Terrain.WATER:
                    return "Cold lava";
                case Terrain.GRASS:
                    return "Embermoss";
                case Terrain.HIGH_GRASS:
                    return "Emberfungi";
                case Terrain.STATUE:
                case Terrain.STATUE_SP:
                    return "Pillar";
                default:
                    return base.TileName(tile);
            }
        }

        public override string TileDesc(int tile)
        {
            switch (tile)
            {
                case Terrain.WATER:
                    return "It looks like lava, but it's cold and probably safe to touch.";
                case Terrain.STATUE:
                case Terrain.STATUE_SP:
                    return "The pillar is made of real humanoid skulls. Awesome.";
                default:
                    return base.TileDesc(tile);
            }
        }

        public override void AddVisuals(Scene scene)
        {
            HallsLevel.AddVisuals(this, scene);
        }
    }
}