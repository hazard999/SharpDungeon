using Android.Graphics;
using pdsharp.gltextures;
using Java.Nio;
using pdsharp.glwrap;

namespace pdsharp.noosa
{
    public class Image : Visual
    {
        public SmartTexture texture;
        protected internal RectF frame;

        public bool flipHorizontal;
        public bool flipVertical;

        protected internal float[] vertices;
        protected internal FloatBuffer verticesBuffer;

        protected internal bool dirty;

        public Image()
            : base(0, 0, 0, 0)
        {
            vertices = new float[16];
            verticesBuffer = Quad.Create();
        }

        public Image(Image src)
            : this()
        {
            Copy(src);
        }

        public Image(object tx)
            : this()
        {
            Texture(tx);
        }

        public Image(object tx, int Left, int Top, int Width, int Height)
            : this(tx)
        {
            Frame(texture.UvRect(Left, Top, Left + Width, Top + Height));
        }

        public virtual void Texture(object tx)
        {
            texture = tx is SmartTexture ? (SmartTexture)tx : TextureCache.Get(tx);
            Frame(new RectF(0, 0, 1, 1));
        }

        public virtual void Frame(RectF frame)
        {
            this.frame = frame;

            _Width = frame.Width() * texture.Width;
            _Height = frame.Height() * texture.Height;

            UpdateFrame();
            updateVertices();
        }

        public virtual void Frame(int Left, int Top, int Width, int Height)
        {
            Frame(texture.UvRect(Left, Top, Left + Width, Top + Height));
        }

        public virtual RectF Frame()
        {
            return new RectF(frame);
        }

        public virtual void Copy(Image other)
        {
            texture = other.texture;
            frame = new RectF(other.frame);

            _Width = other.Width;
            _Height = other.Height;

            UpdateFrame();
            updateVertices();
        }

        protected internal virtual void UpdateFrame()
        {

            if (flipHorizontal)
            {
                vertices[2] = frame.Right;
                vertices[6] = frame.Left;
                vertices[10] = frame.Left;
                vertices[14] = frame.Right;
            }
            else
            {
                vertices[2] = frame.Left;
                vertices[6] = frame.Right;
                vertices[10] = frame.Right;
                vertices[14] = frame.Left;
            }

            if (flipVertical)
            {
                vertices[3] = frame.Bottom;
                vertices[7] = frame.Bottom;
                vertices[11] = frame.Top;
                vertices[15] = frame.Top;
            }
            else
            {
                vertices[3] = frame.Top;
                vertices[7] = frame.Top;
                vertices[11] = frame.Bottom;
                vertices[15] = frame.Bottom;
            }

            dirty = true;
        }

        protected internal virtual void updateVertices()
        {

            vertices[0] = 0;
            vertices[1] = 0;

            vertices[4] = Width;
            vertices[5] = 0;

            vertices[8] = Width;
            vertices[9] = Height;

            vertices[12] = 0;
            vertices[13] = Height;

            dirty = true;
        }

        public override void Draw()
        {
            base.Draw();

            var script = NoosaScript.Get();

            texture.Bind();

            script.Camera(Camera);

            script.UModel.valueM4(Matrix);
            script.Lighting(Rm, Gm, Bm, Am, RA, Ga, Ba, Aa);

            if (dirty)
            {
                verticesBuffer.Position(0);
                verticesBuffer.Put(vertices);
                dirty = false;
            }
            script.DrawQuad(verticesBuffer);
        }
    }
}