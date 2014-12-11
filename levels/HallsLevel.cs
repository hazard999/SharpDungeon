using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.noosa;
using pdsharp.noosa.particles;
using sharpdungeon.items;
using System;
using System.Linq;

namespace sharpdungeon.levels
{
    public class HallsLevel : RegularLevel
    {
        public HallsLevel()
        {
            MinRoomSize = 6;

            viewDistance = Math.Max(25 - Dungeon.Depth, 1);

            color1 = 0x801500;
            color2 = 0xa68521;
        }

        public override void Create()
        {
            AddItemToSpawn(new Torch());
            base.Create();
        }

        public override string TilesTex()
        {
            return Assets.TILES_HALLS;
        }

        public override string WaterTex()
        {
            return Assets.WATER_HALLS;
        }

        protected internal override bool[] Water()
        {
            return Patch.Generate(feeling == Feeling.WATER ? 0.55f : 0.40f, 6);
        }

        protected internal override bool[] Grass()
        {
            return Patch.Generate(feeling == Feeling.GRASS ? 0.55f : 0.30f, 3);
        }

        protected internal override void Decorate()
        {
            for (var i = Width + 1; i < Length - Width - 1; i++)
            {
                switch (map[i])
                {
                    case Terrain.EMPTY:
                        {
                            var count = NEIGHBOURS8.Count(neighbour => (Terrain.Flags[map[i + neighbour]] & Terrain.PASSABLE) > 0);

                            if (pdsharp.utils.Random.Int(80) < count)
                                map[i] = Terrain.EMPTY_DECO;
                        }
                        break;
                    case Terrain.WALL:
                        {
                            var count = NEIGHBOURS4.Count(neighbour => map[i + neighbour] == Terrain.WATER);

                            if (pdsharp.utils.Random.Int(4) < count)
                                map[i] = Terrain.WALL_DECO;
                        }
                        break;
                }
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
                case Terrain.BOOKSHELF:
                    return "Books in ancient languages smoulder in the bookshelf.";
                default:
                    return base.TileDesc(tile);
            }
        }

        public override void AddVisuals(Scene scene)
        {
            base.AddVisuals(scene);
            AddVisuals(this, scene);
        }

        public static void AddVisuals(Level level, Scene scene)
        {
            for (var i = 0; i < Length; i++)
                if (level.map[i] == 63)
                    scene.Add(new Stream(i));
        }

        private class Stream : Group
        {
            private readonly int _pos;

            private float _delay;

            public Stream(int pos)
            {
                _pos = pos;

                _delay = pdsharp.utils.Random.Float(2);
            }

            public override void Update()
            {
                Visible = Dungeon.Visible[_pos];

                if (!Visible)
                    return;

                base.Update();

                if (!((_delay -= Game.Elapsed) <= 0))
                    return;

                _delay = pdsharp.utils.Random.Float(2);

                var p = DungeonTilemap.TileToWorld(_pos);
                Recycle<FireParticle>().Reset(p.X + pdsharp.utils.Random.Float(DungeonTilemap.Size), p.Y + pdsharp.utils.Random.Float(DungeonTilemap.Size));
            }

            public override void Draw()
            {
                GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
                base.Draw();
                GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
            }
        }

        public class FireParticle : PixelParticle.Shrinking
        {

            public FireParticle()
            {
                Color(0xEE7722);
                Lifespan = 1f;

                Acc.Set(0, +80);
            }

            public virtual void Reset(float x, float y)
            {
                Revive();

                X = x;
                Y = y;

                Left = Lifespan;

                Speed.Set(0, -40);
                Size(4);
            }

            public override void Update()
            {
                base.Update();
                var p = Left / Lifespan;
                Am = p > 0.8f ? (1 - p) * 5 : 1;
            }
        }
    }
}