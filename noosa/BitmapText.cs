using Android.Graphics;
using Java.Nio;
using pdsharp.gltextures;
using pdsharp.glwrap;

namespace pdsharp.noosa
{
    public class BitmapText : Visual
    {
        protected internal string text;
        protected internal Font font;

        protected internal float[] Vertices = new float[16];
        protected internal FloatBuffer Quads;

        public int RealLength;

        protected internal bool Dirty = true;

        public BitmapText()
            : this("", null)
        {
        }

        public BitmapText(Font font)
            : this("", font)
        {
        }

        public BitmapText(string text, Font font)
            : base(0, 0, 0, 0)
        {
            this.text = text;
            this.font = font;
        }

        public override void Destroy()
        {
            text = null;
            font = null;
            Vertices = null;
            Quads = null;
            base.Destroy();
        }

        protected override void UpdateMatrix()
        {
            // "origin" field is ignored
            glwrap.Matrix.Identity = Matrix;
            glwrap.Matrix.Translate(Matrix, X, Y);
            glwrap.Matrix.Scale(Matrix, Scale.X, Scale.Y);
            glwrap.Matrix.Rotate(Matrix, Angle);
        }

        public override void Draw()
        {
            base.Draw();

            var script = NoosaScript.Get();

            font.Texture.Bind();

            if (Dirty)
                UpdateVertices();

            script.Camera(Camera);

            script.UModel.valueM4(Matrix);
            script.Lighting(Rm, Gm, Bm, Am, RA, Ga, Ba, Aa);
            script.DrawQuadSet(Quads, RealLength);
        }

        protected internal virtual void UpdateVertices()
        {
            _Width = 0;
            _Height = 0;

            if (text == null)
                text = "";

            Quads = Quad.CreateSet(text.Length);
            RealLength = 0;

            var length = text.Length;
            for (var i = 0; i < length; i++)
            {
                var rect = ((TextureFilm)font).Get(text[i]);

                var w = font.Width(rect);
                var h = font.Height(rect);

                Vertices[0] = Width;
                Vertices[1] = 0;

                Vertices[2] = rect.Left;
                Vertices[3] = rect.Top;

                Vertices[4] = Width + w;
                Vertices[5] = 0;

                Vertices[6] = rect.Right;
                Vertices[7] = rect.Top;

                Vertices[8] = Width + w;
                Vertices[9] = h;

                Vertices[10] = rect.Right;
                Vertices[11] = rect.Bottom;

                Vertices[12] = Width;
                Vertices[13] = h;

                Vertices[14] = rect.Left;
                Vertices[15] = rect.Bottom;

                Quads.Put(Vertices);
                RealLength++;

                _Width += w + font.tracking;
                if (h > Height)
                    _Height = h;
            }

            if (length > 0)
                _Width -= font.tracking;

            Dirty = false;
        }

        public virtual void Measure()
        {
            _Width = 0;
            _Height = 0;

            if (text == null)
                text = "";

            var length = text.Length;
            for (var i = 0; i < length; i++)
            {
                var rect = ((TextureFilm)font).Get(text[i]);

                var w = font.Width(rect);
                var h = font.Height(rect);

                _Width += w + font.tracking;
                if (h > _Height)
                    _Height = h;
            }

            if (length > 0)
                _Width -= font.tracking;
        }

        public virtual float BaseLine()
        {
            return font.BaseLine * Scale.Y;
        }

        public virtual Font Font
        {
            get { return font; }
            set { font = value; }
        }

        public virtual string Text()
        {
            return text;
        }

        public virtual void Text(string str)
        {
            text = str;
            Dirty = true;
        }
    }

    public class Font : TextureFilm
    {
        public const string LatinUpper = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public const string LatinFull = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~\u007F";

        public SmartTexture Texture;

        public float tracking = 0;

        public float BaseLine;

        public bool AutoUppercase = false;

        public float LineHeight;

        protected internal Font(SmartTexture tx)
            : base(tx)
        {
            Texture = tx;
        }

        public Font(SmartTexture tx, int width, string chars)
            : this(tx, width, tx.Height, chars)
        {
        }

        public Font(SmartTexture tx, int width, int height, string chars)
            : base(tx)
        {
            Texture = tx;

            AutoUppercase = chars.Equals(LatinUpper);

            var length = chars.Length;

            var uw = (float)width / tx.Width;
            var vh = (float)height / tx.Height;

            var left = 0;
            var top = 0;
            var bottom = vh;

            for (var i = 0; i < length; i++)
            {
                left += (int)uw;
                var rect = new RectF(left, top, left, bottom);
                Add(chars[i], rect);

                if (left < 1)
                    continue;

                left = 0;
                top = (int)bottom;
                bottom += vh;
            }

            LineHeight = BaseLine = height;
        }

        protected internal virtual void SplitBy(Bitmap bitmap, int height, int color, string chars)
        {
            AutoUppercase = chars.Equals(LatinUpper);
            var length = chars.Length;

            var width = bitmap.Width;
            var vHeight = (float)height / bitmap.Height;

            var pos = GetSpaceMeasuringPos(bitmap, height, color, width);

            Add(' ', new RectF(0, 0, (float)pos / width, vHeight));

            for (var i = 0; i < length; i++)
            {
                var ch = chars[i];
                if (ch == ' ')
                    continue;

                bool found;
                var separator = pos;

                do
                {
                    if (++separator >= width)
                        break;

                    found = true;
                    for (var j = 0; j < height; j++)
                    {
                        if (bitmap.GetPixel(separator, j) == color)
                            continue;

                        found = false;
                        break;
                    }
                }
                while (!found);

                Add(ch, new RectF((float)pos / width, 0, (float)separator / width, vHeight));
                pos = separator + 1;
            }

            LineHeight = BaseLine = Height(Frames[chars[0]]);
        }

        private static int GetSpaceMeasuringPos(Bitmap bitmap, int height, int color, int width)
        {
            for (var pos = 0; pos < width; pos++)
                for (var j = 0; j < height; j++)
                    if (bitmap.GetPixel(pos, j) != color)
                        return pos;
            return -1;
        }

        public static Font ColorMarked(Bitmap bmp, int color, string chars)
        {
            var font = new Font(TextureCache.Get(bmp));
            font.SplitBy(bmp, bmp.Height, color, chars);
            return font;
        }

        public static Font ColorMarked(Bitmap bmp, int height, int color, string chars)
        {
            var font = new Font(TextureCache.Get(bmp));
            font.SplitBy(bmp, height, color, chars);
            return font;
        }

        public virtual RectF Get(char ch)
        {
            return base.Get(AutoUppercase ? char.ToUpper(ch) : ch);
        }
    }
}