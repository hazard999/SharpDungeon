using System.Linq;
using pdsharp.noosa.particles;
using sharpdungeon.actors.mobs.npcs;
using pdsharp.noosa;
using pdsharp.utils;

namespace sharpdungeon.levels
{
    public class CityLevel : RegularLevel
    {
        public CityLevel()
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

        protected internal override bool[] Water()
        {
            return Patch.Generate(feeling == Feeling.WATER ? 0.65f : 0.45f, 4);
        }

        protected internal override bool[] Grass()
        {
            return Patch.Generate(feeling == Feeling.GRASS ? 0.60f : 0.40f, 3);
        }

        protected internal override void AssignRoomType()
        {
            base.AssignRoomType();

            foreach (Room r in Rooms.Where(r => r.type == RoomType.TUNNEL))
                r.type = RoomType.PASSAGE;
        }

        protected internal override void Decorate()
        {
            for (var i = 0; i < Length; i++)
            {
                if (map[i] == Terrain.EMPTY && pdsharp.utils.Random.Int(10) == 0)
                    map[i] = Terrain.EMPTY_DECO;
                else
                    if (map[i] == Terrain.WALL && pdsharp.utils.Random.Int(8) == 0)
                        map[i] = Terrain.WALL_DECO;
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

        protected internal override void CreateItems()
        {
            base.CreateItems();

            Imp.Quest.Spawn(this, RoomEntrance);
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
                    return "A ramp leads down to the lower depth.";
                case Terrain.WALL_DECO:
                case Terrain.EMPTY_DECO:
                    return "Several tiles are missing here.";
                case Terrain.EMPTY_SP:
                    return "Thick carpet covers the floor.";
                case Terrain.STATUE:
                case Terrain.STATUE_SP:
                    return "The statue depicts some dwarf standing in a heroic stance.";
                case Terrain.BOOKSHELF:
                    return "The rows of books on different disciplines fill the bookshelf.";
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
                if (level.map[i] == Terrain.WALL_DECO)
                    scene.Add(new Smoke(i));
        }

        private class Smoke : Emitter
        {
            private readonly int _pos;

            private static readonly Factory Factory = new CityLevelSmokeEmitterFactory();

            public Smoke(int pos)
            {
                _pos = pos;

                var p = DungeonTilemap.TileCenterToWorld(pos);
                Pos(p.X - 4, p.Y - 2, 4, 0);

                Pour(Factory, 0.2f);
            }

            public override void Update()
            {
                Visible = Dungeon.Visible[_pos];
                if (Visible)
                    base.Update();
            }
        }

        public sealed class SmokeParticle : PixelParticle
        {
            public SmokeParticle()
            {
                Color(0x000000);
                Speed.Set(Random.Float(8), -Random.Float(8));
            }

            public void Reset(float x, float y)
            {
                Revive();

                X = x;
                Y = y;

                Left = Lifespan = 2f;
            }

            public override void Update()
            {
                base.Update();
                var p = Left / Lifespan;
                Am = p > 0.8f ? 1 - p : p * 0.25f;
                Size(8 - p * 4);
            }
        }
    }

    internal class CityLevelSmokeEmitterFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            var positive = emitter.Recycle<CityLevel.SmokeParticle>();
            positive.Reset(x, y);
        }
    }
}