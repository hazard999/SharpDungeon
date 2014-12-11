using Android.Graphics;
using Java.Util;
using pdsharp.gltextures;
using pdsharp.noosa;
using sharpdungeon.scenes;

namespace sharpdungeon
{
    public class FogOfWar : Image
    {
        private readonly Color _visibleColor = Android.Graphics.Color.Argb(0x00, 0x00, 0x00, 0x00);
        private readonly Color _visited = Android.Graphics.Color.Argb(0xcc, 0x11, 0x11, 0x11);
        private readonly Color _mapped = Android.Graphics.Color.Argb(0xcc, 0x44, 0x22, 0x11);
        private readonly Color _invisible = Android.Graphics.Color.Argb(0xFF, 0x00, 0x00, 0x00);

        private int[] _pixels;

        private readonly int _pWidth;
        private readonly int _pHeight;

        private readonly int _width2;
        private readonly int _height2;

        public FogOfWar(int mapWidth, int mapHeight)
        {
            _pWidth = mapWidth + 1;
            _pHeight = mapHeight + 1;

            _width2 = 1;
            while (_width2 < _pWidth)
                _width2 <<= 1;

            _height2 = 1;
            while (_height2 < _pHeight)
                _height2 <<= 1;

            const float size = DungeonTilemap.Size;
            _Width = _width2 * size;
            _Height = _height2 * size;

            Texture(new FogTexture(_width2, _height2));

            Scale.Set(DungeonTilemap.Size, DungeonTilemap.Size);

            X = Y = -size / 2;
        }

        public virtual void UpdateVisibility(bool[] visible, bool[] visited, bool[] mapped)
        {
            if (_pixels == null)
            {
                _pixels = new int[_width2 * _height2];
                Arrays.Fill(_pixels, _invisible);
            }

            for (var i = 1; i < _pHeight - 1; i++)
            {
                var pos = (_pWidth - 1) * i;
                for (var j = 1; j < _pWidth - 1; j++)
                {
                    pos++;
                    var c = _invisible;

                    if (visible[pos] && visible[pos - (_pWidth - 1)] && visible[pos - 1] && visible[pos - (_pWidth - 1) - 1])
                        c = _visibleColor;
                    else if (visited[pos] && visited[pos - (_pWidth - 1)] && visited[pos - 1] && visited[pos - (_pWidth - 1) - 1])
                        c = _visited;
                    else if (mapped[pos] && mapped[pos - (_pWidth - 1)] && mapped[pos - 1] && mapped[pos - (_pWidth - 1) - 1])
                        c = _mapped;

                    _pixels[i * _width2 + j] = c;
                }
            }

            texture.Pixels(_width2, _height2, _pixels);
        }

        private class FogTexture : SmartTexture
        {
            public FogTexture(int width2, int height2)
                : base(Android.Graphics.Bitmap.CreateBitmap(width2, height2, Android.Graphics.Bitmap.Config.Argb8888))
            {
                Filter(Linear, Linear);
                TextureCache.Add(typeof(FogOfWar), this);
            }

            public override void Reload()
            {
                base.Reload();
                GameScene.AfterObserve();
            }
        }
    }

}