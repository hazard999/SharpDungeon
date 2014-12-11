using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.items.keys;
using sharpdungeon.levels.painters;
using sharpdungeon.scenes;

namespace sharpdungeon.levels
{
    public class CavesBossLevel : Level
    {
        public CavesBossLevel()
        {
            color1 = 0x534f3e;
            color2 = 0xb9d661;

            viewDistance = 6;
        }

        private const int ROOM_LEFT = Width / 2 - 2;
        private const int ROOM_RIGHT = Width / 2 + 2;
        private const int ROOM_TOP = Height / 2 - 2;
        private const int ROOM_BOTTOM = Height / 2 + 2;

        private int arenaDoor;
        private bool enteredArena = false;
        private bool keyDropped = false;

        public override string TilesTex()
        {
            return Assets.TILES_CAVES;
        }

        public override string WaterTex()
        {
            return Assets.WATER_CAVES;
        }

        private const string DOOR = "door";
        private const string ENTERED = "entered";
        private const string DROPPED = "droppped";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(DOOR, arenaDoor);
            bundle.Put(ENTERED, enteredArena);
            bundle.Put(DROPPED, keyDropped);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            arenaDoor = bundle.GetInt(DOOR);
            enteredArena = bundle.GetBoolean(ENTERED);
            keyDropped = bundle.GetBoolean(DROPPED);
        }

        protected override bool Build()
        {
            var topMost = int.MaxValue;

            for (var i = 0; i < 8; i++)
            {
                int left, right, top, bottom;
                if (Random.Int(2) == 0)
                {
                    left = Random.Int(1, ROOM_LEFT - 3);
                    right = ROOM_RIGHT + 3;
                }
                else
                {
                    left = ROOM_LEFT - 3;
                    right = Random.Int(ROOM_RIGHT + 3, Width - 1);
                }
                if (Random.Int(2) == 0)
                {
                    top = Random.Int(2, ROOM_TOP - 3);
                    bottom = ROOM_BOTTOM + 3;
                }
                else
                {
                    top = ROOM_LEFT - 3;
                    bottom = Random.Int(ROOM_TOP + 3, Height - 1);
                }

                Painter.Fill(this, left, top, right - left + 1, bottom - top + 1, Terrain.EMPTY);

                if (top >= topMost)
                    continue;

                topMost = top;
                exit = Random.Int(left, right) + (top - 1) * Width;
            }

            map[exit] = Terrain.LOCKED_EXIT;

            for (var i = 0; i < Length; i++)
            {
                if (map[i] == Terrain.EMPTY && Random.Int(6) == 0)
                    map[i] = Terrain.INACTIVE_TRAP;
            }

            Painter.Fill(this, ROOM_LEFT - 1, ROOM_TOP - 1, ROOM_RIGHT - ROOM_LEFT + 3, ROOM_BOTTOM - ROOM_TOP + 3, Terrain.WALL);
            Painter.Fill(this, ROOM_LEFT, ROOM_TOP + 1, ROOM_RIGHT - ROOM_LEFT + 1, ROOM_BOTTOM - ROOM_TOP, Terrain.EMPTY);

            Painter.Fill(this, ROOM_LEFT, ROOM_TOP, ROOM_RIGHT - ROOM_LEFT + 1, 1, Terrain.TOXIC_TRAP);

            arenaDoor = Random.Int(ROOM_LEFT, ROOM_RIGHT) + (ROOM_BOTTOM + 1) * Width;
            map[arenaDoor] = Terrain.DOOR;

            entrance = Random.Int(ROOM_LEFT + 1, ROOM_RIGHT - 1) + Random.Int(ROOM_TOP + 1, ROOM_BOTTOM - 1) * Width;
            map[entrance] = Terrain.ENTRANCE;

            var patch = Patch.Generate(0.45f, 6);
            for (var i = 0; i < Length; i++)
                if (map[i] == Terrain.EMPTY && patch[i])
                    map[i] = Terrain.WATER;

            return true;
        }

        protected internal override void Decorate()
        {
            for (var i = Width + 1; i < Length - Width; i++)
            {
                if (map[i] != Terrain.EMPTY)
                    continue;

                var n = 0;

                if (map[i + 1] == Terrain.WALL)
                    n++;

                if (map[i - 1] == Terrain.WALL)
                    n++;

                if (map[i + Width] == Terrain.WALL)
                    n++;

                if (map[i - Width] == Terrain.WALL)
                    n++;

                if (Random.Int(8) <= n)
                    map[i] = Terrain.EMPTY_DECO;
            }

            for (var i = 0; i < Length; i++)
                if (map[i] == Terrain.WALL && Random.Int(8) == 0)
                    map[i] = Terrain.WALL_DECO;

            int sign;
            do
            {
                sign = Random.Int(ROOM_LEFT, ROOM_RIGHT) + Random.Int(ROOM_TOP, ROOM_BOTTOM) * Width;
            }
            while (sign == entrance);

            map[sign] = Terrain.SIGN;
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
                pos = Random.IntRange(ROOM_LEFT, ROOM_RIGHT) + Random.IntRange(ROOM_TOP + 1, ROOM_BOTTOM) * Width;
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

            if (enteredArena || !outsideEntraceRoom(cell) || hero != Dungeon.Hero)
                return;

            enteredArena = true;

            var boss = Bestiary.Mob(Dungeon.Depth);
            boss.State = boss.HUNTING;
            do
            {
                boss.pos = Random.Int(Length);
            }
            while (!passable[boss.pos] || !outsideEntraceRoom(boss.pos) || Dungeon.Visible[boss.pos]);

            GameScene.Add(boss);

            Set(arenaDoor, Terrain.WALL);
            GameScene.UpdateMap(arenaDoor);
            Dungeon.Observe();

            CellEmitter.Get(arenaDoor).Start(Speck.Factory(Speck.ROCK), 0.07f, 10);
            Camera.Main.Shake(3, 0.7f);
            Sample.Instance.Play(Assets.SND_ROCKS);
        }

        public override Heap Drop(Item item, int cell)
        {
            if (keyDropped || !(item is SkeletonKey)) 
                return base.Drop(item, cell);

            keyDropped = true;

            CellEmitter.Get(arenaDoor).Start(Speck.Factory(Speck.ROCK), 0.07f, 10);

            Set(arenaDoor, Terrain.EMPTY_DECO);
            GameScene.UpdateMap(arenaDoor);
            Dungeon.Observe();

            return base.Drop(item, cell);
        }

        private bool outsideEntraceRoom(int cell)
        {
            var cx = cell % Width;
            var cy = cell / Width;
            return cx < ROOM_LEFT - 1 || cx > ROOM_RIGHT + 1 || cy < ROOM_TOP - 1 || cy > ROOM_BOTTOM + 1;
        }

        public override string TileName(int tile)
        {
            switch (tile)
            {
                case Terrain.GRASS:
                    return "Fluorescent moss";
                case Terrain.HIGH_GRASS:
                    return "Fluorescent mushrooms";
                case Terrain.WATER:
                    return "Freezing cold water.";
                default:
                    return base.TileName(tile);
            }
        }

        public override string TileDesc(int tile)
        {
            switch (tile)
            {
                case Terrain.ENTRANCE:
                    return "The ladder leads up to the upper depth.";
                case Terrain.EXIT:
                    return "The ladder leads down to the lower depth.";
                case Terrain.HIGH_GRASS:
                    return "Huge mushrooms block the view.";
                case Terrain.WALL_DECO:
                    return "A vein of some ore is visible on the wall. Gold?";
                default:
                    return base.TileDesc(tile);
            }
        }

        public override void AddVisuals(Scene scene)
        {
            CavesLevel.AddVisuals(this, scene);
        }
    }
}