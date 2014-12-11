using Java.Util;
using pdsharp.noosa;
using sharpdungeon.items;
using sharpdungeon.levels.painters;

namespace sharpdungeon.levels
{
    public class LastLevel : Level
    {
        private const int Size = 7;

        public LastLevel()
        {
            color1 = 0x801500;
            color2 = 0xa68521;
        }

        private int _pedestal;

        public override string TilesTex()
        {
            return Assets.TILES_HALLS;
        }

        public override string WaterTex()
        {
            return Assets.WATER_HALLS;
        }

        protected override bool Build()
        {
            Arrays.Fill(map, Terrain.WALL);
            Painter.Fill(this, 1, 1, Size, Size, Terrain.WATER);
            Painter.Fill(this, 2, 2, Size - 2, Size - 2, Terrain.EMPTY);
            Painter.Fill(this, Size / 2, Size / 2, 3, 3, Terrain.EMPTY_SP);

            entrance = Size * Width + Size / 2 + 1;
            map[entrance] = Terrain.ENTRANCE;

            exit = entrance - Width * Size;
            map[exit] = Terrain.LOCKED_EXIT;

            _pedestal = (Size / 2 + 1) * (Width + 1);
            map[_pedestal] = Terrain.PEDESTAL;
            map[_pedestal - 1] = map[_pedestal + 1] = Terrain.STATUE_SP;

            feeling = Feeling.NONE;

            return true;
        }

        protected internal override void Decorate()
        {
            for (var i = 0; i < Length; i++)
                if (map[i] == Terrain.EMPTY && pdsharp.utils.Random.Int(10) == 0)
                    map[i] = Terrain.EMPTY_DECO;
        }

        protected internal override void CreateMobs()
        {
        }

        protected internal override void CreateItems()
        {
            Drop(new Amulet(), _pedestal);
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