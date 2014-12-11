using System.Collections.Generic;
using Android.Graphics;
using pdsharp.gltextures;

namespace pdsharp.noosa
{
    public class TextureFilm
    {
        private static readonly RectF Full = new RectF(0, 0, 1, 1);

        private readonly int _texWidth;
        private readonly int _texHeight;

        internal Dictionary<object, RectF> Frames = new Dictionary<object, RectF>();

        public TextureFilm(object tx)
        {
            var texture = TextureCache.Get(tx);

            _texWidth = texture.Width;
            _texHeight = texture.Height;

            Add("", Full);
        }

        public TextureFilm(SmartTexture texture, int Width)
            : this(texture, Width, texture.Height)
        {
        }

        public TextureFilm(object tx, int Width, int Height)
        {
            var texture = TextureCache.Get(tx);

            _texWidth = texture.Width;
            _texHeight = texture.Height;

            var uw = (float)Width / _texWidth;
            var vh = (float)Height / _texHeight;
            var cols = _texWidth / Width;
            var rows = _texHeight / Height;

            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                {
                    var rect = new RectF(j*uw, i*vh, (j + 1)*uw, (i + 1)*vh);
                    Add(i*cols + j, rect);
                }
        }

        public TextureFilm(TextureFilm atlas, object key, int width, int height)
        {
            _texWidth = atlas._texWidth;
            _texHeight = atlas._texHeight;

            var patch = atlas.Get(key);

            var uw = (float)width / _texWidth;
            var vh = (float)height / _texHeight;
            var cols = (int)Width(patch) / width;
            var rows = (int)Height(patch) / height;

            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                {
                    var rect = new RectF(j * uw, i * vh, (j + 1) * uw, (i + 1) * vh);
                    rect.Offset(patch.Left, patch.Top);
                    Add(i * cols + j, rect);
                }
        }

        public void Add(object id, RectF rect)
        {
            Frames.Add(id, rect);
        }

        public RectF Get(object id)
        {
            return Frames[id];
        }

        public float Width(RectF frame)
        {
            return frame.Width() * _texWidth;
        }

        public float Height(RectF frame)
        {
            return frame.Height() * _texHeight;
        }
    }
}