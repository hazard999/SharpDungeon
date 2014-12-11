using System.Linq;
using pdsharp.noosa;
using pdsharp.noosa.particles;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;

namespace sharpdungeon.levels
{
    public class PrisonLevel : RegularLevel
    {
        public PrisonLevel()
        {
            color1 = 0x6a723d;
            color2 = 0x88924c;
        }

        public override string TilesTex()
        {
            return Assets.TILES_PRISON;
        }

        public override string WaterTex()
        {
            return Assets.WATER_PRISON;
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

            foreach (var r in Rooms.Where(r => r.type == RoomType.TUNNEL))
                r.type = RoomType.PASSAGE;
        }

        protected internal override void CreateMobs()
        {
            base.CreateMobs();

            Wandmaker.Quest.Spawn(this, RoomEntrance);
        }

        protected internal override void Decorate()
        {

            for (var i = Width + 1; i < Length - Width - 1; i++)
            {
                if (map[i] != Terrain.EMPTY)
                    continue;

                var c = 0.05f;
                if (map[i + 1] == Terrain.WALL && map[i + Width] == Terrain.WALL)
                    c += 0.2f;
                if (map[i - 1] == Terrain.WALL && map[i + Width] == Terrain.WALL)
                    c += 0.2f;
                if (map[i + 1] == Terrain.WALL && map[i - Width] == Terrain.WALL)
                    c += 0.2f;
                if (map[i - 1] == Terrain.WALL && map[i - Width] == Terrain.WALL)
                    c += 0.2f;

                if (pdsharp.utils.Random.Float() < c)
                    map[i] = Terrain.EMPTY_DECO;
            }

            for (var i = 0; i < Width; i++)
                if (map[i] == Terrain.WALL && (map[i + Width] == Terrain.EMPTY || map[i + Width] == Terrain.EMPTY_SP) && pdsharp.utils.Random.Int(6) == 0)
                    map[i] = Terrain.WALL_DECO;

            for (var i = Width; i < Length - Width; i++)
                if (map[i] == Terrain.WALL && map[i - Width] == Terrain.WALL && (map[i + Width] == Terrain.EMPTY || map[i + Width] == Terrain.EMPTY_SP) && pdsharp.utils.Random.Int(3) == 0)
                    map[i] = Terrain.WALL_DECO;

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
                case Terrain.BOOKSHELF:
                    return "This is probably a vestige of a prison library. Might it burn?";
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
                    scene.Add(new Torch(i));
        }

        private class Torch : Emitter
        {
            private readonly int _pos;

            public Torch(int pos)
            {
                _pos = pos;

                var p = DungeonTilemap.TileCenterToWorld(pos);
                Pos(p.X - 1, p.Y + 3, 2, 0);

                Pour(FlameParticle.Factory, 0.15f);

                Add(new Halo(16, 0xFFFFCC, 0.2f).Point(p.X, p.Y));
            }

            public override void Update()
            {
                Visible = Dungeon.Visible[_pos];
                if (Visible)
                    base.Update();
            }
        }
    }
}