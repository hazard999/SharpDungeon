using Java.Util;

namespace sharpdungeon.levels
{
    public class DeadEndLevel : Level
    {
        private const int Size = 5;

        public DeadEndLevel()
        {
            color1 = 0x534f3e;
            color2 = 0xb9d661;
        }

        public override string TilesTex()
        {
            return Assets.TILES_CAVES;
        }

        public override string WaterTex()
        {
            return Assets.WATER_HALLS;
        }

        protected override bool Build()
        {
            Arrays.Fill(map, Terrain.WALL);

            for (var i = 2; i < Size; i++)
                for (var j = 2; j < Size; j++)
                    map[i * Width + j] = Terrain.EMPTY;

            for (var i = 1; i <= Size; i++)
                map[Width + i] = map[Width * Size + i] = map[Width * i + 1] = map[Width * i + Size] = Terrain.WATER;

            entrance = Size * Width + Size / 2 + 1;
            map[entrance] = Terrain.ENTRANCE;

            exit = -1;

            map[(Size / 2 + 1) * (Width + 1)] = Terrain.SIGN;

            return true;
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
        }

        protected internal override void CreateMobs()
        {
        }

        protected internal override void CreateItems()
        {
        }

        public override int RandomRespawnCell()
        {
            return -1;
        }
    }
}