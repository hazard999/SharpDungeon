using pdsharp.noosa;
using pdsharp.noosa.particles;
using pdsharp.utils;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.levels.painters;

namespace sharpdungeon.levels
{
    public class CavesLevel : RegularLevel
    {
        public CavesLevel()
        {
            color1 = 0x534f3e;
            color2 = 0xb9d661;

            viewDistance = 6;
        }

        public override string TilesTex()
        {
            return Assets.TILES_CAVES;
        }

        public override string WaterTex()
        {
            return Assets.WATER_CAVES;
        }

        protected internal override bool[] Water()
        {
            return Patch.Generate(feeling == Feeling.WATER ? 0.60f : 0.45f, 6);
        }

        protected internal override bool[] Grass()
        {
            return Patch.Generate(feeling == Feeling.GRASS ? 0.55f : 0.35f, 3);
        }

        protected internal override void AssignRoomType()
        {
            base.AssignRoomType();

            Blacksmith.Quest.Spawn(Rooms);
        }

        protected internal override void Decorate()
        {
            foreach (var room in Rooms)
            {
                if (room.type != RoomType.STANDARD)
                    continue;

                if (room.Width() <= 3 || room.Height() <= 3)
                    continue;

                var s = room.Square();

                if (Random.Int(s) > 8)
                {
                    var corner = (room.Left + 1) + (room.Top + 1) * Width;
                    if (map[corner - 1] == Terrain.WALL && map[corner - Width] == Terrain.WALL)
                        map[corner] = Terrain.WALL;
                }

                if (Random.Int(s) > 8)
                {
                    var corner = (room.Right - 1) + (room.Top + 1) * Width;
                    if (map[corner + 1] == Terrain.WALL && map[corner - Width] == Terrain.WALL)
                        map[corner] = Terrain.WALL;
                }

                if (Random.Int(s) > 8)
                {
                    var corner = (room.Left + 1) + (room.Bottom - 1) * Width;
                    if (map[corner - 1] == Terrain.WALL && map[corner + Width] == Terrain.WALL)
                        map[corner] = Terrain.WALL;
                }

                if (Random.Int(s) > 8)
                {
                    var corner = (room.Right - 1) + (room.Bottom - 1) * Width;
                    if (map[corner + 1] == Terrain.WALL && map[corner + Width] == Terrain.WALL)
                        map[corner] = Terrain.WALL;
                }

                foreach (var n in room.Connected.Keys)
                {
                    if ((n.type == RoomType.STANDARD || n.type == RoomType.TUNNEL) && Random.Int(3) == 0)
                        Painter.Set(this, room.Connected[n], Terrain.EMPTY_DECO);
                }
            }

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

                if (Random.Int(6) <= n)
                    map[i] = Terrain.EMPTY_DECO;
            }

            for (var i = 0; i < Length; i++)
            {
                if (map[i] == Terrain.WALL && Random.Int(12) == 0)
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

            if (Dungeon.BossLevel(Dungeon.Depth + 1))
                return;

            foreach (var r in Rooms)
            {
                if (r.type != RoomType.STANDARD)
                    continue;

                foreach (var n in r.Neigbours)
                {
                    if (n.type != RoomType.STANDARD || r.Connected.ContainsKey(n))
                        continue;

                    var w = r.Intersect(n);
                    if (w.Left == w.Right && w.Bottom - w.Top >= 5)
                    {
                        w.Top += 2;
                        w.Bottom -= 1;

                        w.Right++;

                        Painter.Fill(this, w.Left, w.Top, 1, w.Height(), Terrain.CHASM);
                    }
                    else
                        if (w.Top == w.Bottom && w.Right - w.Left >= 5)
                        {
                            w.Left += 2;
                            w.Right -= 1;

                            w.Bottom++;

                            Painter.Fill(this, w.Left, w.Top, w.Width(), 1, Terrain.CHASM);
                        }
                }
            }
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
                case Terrain.BOOKSHELF:
                    return "Who would need a bookshelf in a cave?";
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
                    scene.Add(new Vein(i));
        }

        private class Vein : Group
        {
            private readonly int _pos;

            private float _delay;

            public Vein(int pos)
            {
                _pos = pos;

                _delay = pdsharp.utils.Random.Float(2);
            }

            public override void Update()
            {
                if (!(Visible = Dungeon.Visible[_pos]))
                    return;

                base.Update();

                if (!((_delay -= Game.Elapsed) <= 0))
                    return;

                _delay = pdsharp.utils.Random.Float();

                var p = DungeonTilemap.TileToWorld(_pos);
                Recycle<Sparkle>().Reset(p.X + pdsharp.utils.Random.Float(DungeonTilemap.Size), p.Y + pdsharp.utils.Random.Float(DungeonTilemap.Size));
            }
        }

        public sealed class Sparkle : PixelParticle
        {
            public void Reset(float x, float y)
            {
                Revive();

                X = x;
                Y = y;

                Left = Lifespan = 0.5f;
            }

            public override void Update()
            {
                base.Update();

                var p = Left / Lifespan;
                Size((Am = p < 0.5f ? p * 2 : (1 - p) * 2) * 2);
            }
        }
    }
}