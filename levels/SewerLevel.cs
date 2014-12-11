using pdsharp.noosa;
using pdsharp.noosa.particles;
using pdsharp.utils;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items;
using sharpdungeon.scenes;

namespace sharpdungeon.levels
{
    public class SewerLevel : RegularLevel
    {
        public SewerLevel()
        {
            color1 = 0x48763c;
            color2 = 0x59994a;
        }

        public void DebugBuild()
        {
            Build();
        }

        public override string TilesTex()
        {
            return Assets.TILES_SEWERS;
        }

        public override string WaterTex()
        {
            return Assets.WATER_SEWERS;
        }

        protected internal override bool[] Water()
        {
            return Patch.Generate(feeling == Feeling.WATER ? 0.60f : 0.45f, 5);
        }

        protected internal override bool[] Grass()
        {
            return Patch.Generate(feeling == Feeling.GRASS ? 0.60f : 0.40f, 4);
        }

        protected internal override void Decorate()
        {
            for (var i = 0; i < Width; i++)
                if (map[i] == Terrain.WALL && map[i + Width] == Terrain.WATER && Random.Int(4) == 0)
                    map[i] = Terrain.WALL_DECO;

            for (var i = Width; i < Length - Width; i++)
                if (map[i] == Terrain.WALL && map[i - Width] == Terrain.WALL && map[i + Width] == Terrain.WATER && Random.Int(2) == 0)
                    map[i] = Terrain.WALL_DECO;

            for (var i = Width + 1; i < Length - Width - 1; i++)
                if (map[i] == Terrain.EMPTY)
                {
                    var count = (map[i + 1] == Terrain.WALL ? 1 : 0) + (map[i - 1] == Terrain.WALL ? 1 : 0) + (map[i + Width] == Terrain.WALL ? 1 : 0) + (map[i - Width] == Terrain.WALL ? 1 : 0);

                    if (Random.Int(16) < count * count)
                        map[i] = Terrain.EMPTY_DECO;
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

        protected internal override void CreateMobs()
        {
            base.CreateMobs();

            Ghost.Quest.Spawn(this);
        }

        protected internal override void CreateItems()
        {
            if (Dungeon.DewVial && Random.Int(4 - Dungeon.Depth) == 0)
            {
                AddItemToSpawn(new DewVial());
                Dungeon.DewVial = false;
            }

            base.CreateItems();
        }

        public override void AddVisuals(Scene scene)
        {
            base.AddVisuals(scene);
            AddVisuals(this, scene);
        }

        public static void AddVisuals(Level level, Scene scene)
        {
            for (var i = 0; i < Length; i++)
                if (level.map[i] == Terrain.WALL_DECO)
                    scene.Add(new Sink(i));
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
                case Terrain.BOOKSHELF:
                    return "The bookshelf is packed with cheap useless books. Might it burn?";
                default:
                    return base.TileDesc(tile);
            }
        }

        private class Sink : Emitter
        {
            private readonly int _pos;
            private float _rippleDelay;

            private static readonly Factory _factory = new SinkEmitterFactory();

            public Sink(int pos)
            {
                _pos = pos;

                var p = DungeonTilemap.TileCenterToWorld(pos);
                Pos(p.X - 2, p.Y + 1, 4, 0);

                Pour(_factory, 0.05f);
            }

            public override void Update()
            {
                if (!(Visible = Dungeon.Visible[_pos]))
                    return;

                base.Update();

                if (!((_rippleDelay -= Game.Elapsed) <= 0))
                    return;

                GameScene.Ripple((int)(_pos + Width)).Y -= DungeonTilemap.Size / 2;
                _rippleDelay = pdsharp.utils.Random.Float(0.2f, 0.3f);
            }
        }

        public sealed class WaterParticle : PixelParticle
        {
            public WaterParticle()
            {
                Acc.Y = 50;
                Am = 0.5f;

                Color(ColorMath.Random(0xb6ccc2, 0x3b6653));
                Size(2);
            }

            public void Reset(float x, float y)
            {
                Revive();

                X = x;
                Y = y;

                Speed.Set(Random.Float(-2, +2), 0);

                Left = Lifespan = 0.5f;
            }
        }
    }

    internal class SinkEmitterFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            var positive = emitter.Recycle<SewerLevel.WaterParticle>();
            positive.Reset(x, y);
        }
    }
}