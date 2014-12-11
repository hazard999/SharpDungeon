using pdsharp.gltextures;
using pdsharp.noosa;

namespace sharpdungeon.effects
{
    public class CheckedCell : Image
    {
        private float _alpha;

        public CheckedCell(int pos)
            : base(TextureCache.CreateSolid(Android.Graphics.Color.Argb(0xFF, 0x55, 0xAA, 0xFF)))
        {
            Origin.Set(0.5f);

            Point(DungeonTilemap.TileToWorld(pos).Offset(DungeonTilemap.Size / 2, DungeonTilemap.Size / 2));

            _alpha = 0.8f;
        }

        public override void Update()
        {
            if ((_alpha -= Game.Elapsed) > 0)
            {
                Alpha(_alpha);
                Scale.Set(DungeonTilemap.Size * _alpha);
            }
            else
                KillAndErase();
        }
    }
}