using Android.Graphics;
using pdsharp.gltextures;
using pdsharp.glwrap;
using Java.Nio;

namespace pdsharp.noosa
{
    public class Tilemap : Visual
    {
        protected internal SmartTexture texture;
        protected internal TextureFilm tileset;

        protected internal int[] data;
        protected internal int mapWidth;
        protected internal int mapHeight;
        protected internal int size;

        private readonly float _cellW;
        private readonly float _cellH;

        protected internal float[] vertices;
        protected internal FloatBuffer quads;

        public Rect updated;

        public Tilemap(object tx, TextureFilm tileset)
            : base(0, 0, 0, 0)
        {
            texture = TextureCache.Get(tx);
            this.tileset = tileset;

            var r = tileset.Get(0);
            _cellW = tileset.Width(r);
            _cellH = tileset.Height(r);

            vertices = new float[16];

            updated = new Rect();
        }

        public virtual void Map(int[] data, int cols)
        {
            this.data = data;

            mapWidth = cols;
            mapHeight = data.Length / cols;
            size = mapWidth * mapHeight;

            _Width = _cellW * mapWidth;
            _Height = _cellH * mapHeight;

            quads = Quad.CreateSet(size);

            updated.Set(0, 0, mapWidth, mapHeight);
        }

        protected internal virtual void UpdateVertices()
        {
            var y1 = _cellH * updated.Top;
            var y2 = y1 + _cellH;

            for (var i = updated.Top; i < updated.Bottom; i++)
            {
                var x1 = _cellW * updated.Left;
                var x2 = x1 + _cellW;

                var pos = i * mapWidth + updated.Left;
                quads.Position(16 * pos);

                for (var j = updated.Left; j < updated.Right; j++)
                {
                    var uv = tileset.Get(data[pos++]);

                    vertices[0] = x1;
                    vertices[1] = y1;

                    vertices[2] = uv.Left;
                    vertices[3] = uv.Top;

                    vertices[4] = x2;
                    vertices[5] = y1;

                    vertices[6] = uv.Right;
                    vertices[7] = uv.Top;

                    vertices[8] = x2;
                    vertices[9] = y2;

                    vertices[10] = uv.Right;
                    vertices[11] = uv.Bottom;

                    vertices[12] = x1;
                    vertices[13] = y2;

                    vertices[14] = uv.Left;
                    vertices[15] = uv.Bottom;

                    quads.Put(vertices);

                    x1 = x2;
                    x2 += _cellW;
                }

                y1 = y2;
                y2 += _cellH;
            }

            updated.SetEmpty();
        }

        public override void Draw()
        {
            base.Draw();

            var script = NoosaScript.Get();

            texture.Bind();

            script.UModel.valueM4(Matrix);
            script.Lighting(Rm, Gm, Bm, Am, RA, Ga, Ba, Aa);

            if (!updated.IsEmpty)
                UpdateVertices();

            script.Camera(Camera);
            script.DrawQuadSet(quads, size);
        }
    }
}