using Java.Nio;
using pdsharp.gltextures;
using pdsharp.glwrap;

namespace pdsharp.noosa
{
    using RectF = Android.Graphics.RectF;

    public class NinePatch : Visual
    {

        public SmartTexture texture;

        protected internal float[] vertices;
        protected internal FloatBuffer verticesBuffer;

        protected internal RectF outterF;
        protected internal RectF innerF;

        protected internal int marginLeft;
        protected internal int marginRight;
        protected internal int marginTop;
        protected internal int marginBottom;

        protected internal float nWidth;
        protected internal float nHeight;

        public NinePatch(object tx, int margin)
            : this(tx, margin, margin, margin, margin)
        {
        }

        public NinePatch(object tx, int Left, int Top, int Right, int Bottom)
            : this(tx, 0, 0, 0, 0, Left, Top, Right, Bottom)
        {
        }

        public NinePatch(object tx, int x, int y, int w, int h, int margin)
            : this(tx, x, y, w, h, margin, margin, margin, margin)
        {
        }

        public NinePatch(object tx, int x, int y, int w, int h, int Left, int Top, int Right, int Bottom)
            : base(0, 0, 0, 0)
        {

            texture = TextureCache.Get(tx);
            w = w == 0 ? texture.Width : w;
            h = h == 0 ? texture.Height : h;

            nWidth = _Width = w;
            nHeight = _Height = h;

            vertices = new float[16];
            verticesBuffer = Quad.CreateSet(9);

            marginLeft = Left;
            marginRight = Right;
            marginTop = Top;
            marginBottom = Bottom;

            outterF = texture.UvRect(x, y, x + w, y + h);
            innerF = texture.UvRect(x + Left, y + Top, x + w - Right, y + h - Bottom);

            UpdateVertices();
        }

        protected internal virtual void UpdateVertices()
        {
            verticesBuffer.Position(0);

            var Right = Width - marginRight;
            var Bottom = Height - marginBottom;

            Quad.Fill(vertices, 0, marginLeft, 0, marginTop, outterF.Left, innerF.Left, outterF.Top, innerF.Top);
            verticesBuffer.Put(vertices);
            Quad.Fill(vertices, marginLeft, Right, 0, marginTop, innerF.Left, innerF.Right, outterF.Top, innerF.Top);
            verticesBuffer.Put(vertices);
            Quad.Fill(vertices, Right, Width, 0, marginTop, innerF.Right, outterF.Right, outterF.Top, innerF.Top);
            verticesBuffer.Put(vertices);

            Quad.Fill(vertices, 0, marginLeft, marginTop, Bottom, outterF.Left, innerF.Left, innerF.Top, innerF.Bottom);
            verticesBuffer.Put(vertices);
            Quad.Fill(vertices, marginLeft, Right, marginTop, Bottom, innerF.Left, innerF.Right, innerF.Top, innerF.Bottom);
            verticesBuffer.Put(vertices);
            Quad.Fill(vertices, Right, Width, marginTop, Bottom, innerF.Right, outterF.Right, innerF.Top, innerF.Bottom);
            verticesBuffer.Put(vertices);

            Quad.Fill(vertices, 0, marginLeft, Bottom, Height, outterF.Left, innerF.Left, innerF.Bottom, outterF.Bottom);
            verticesBuffer.Put(vertices);
            Quad.Fill(vertices, marginLeft, Right, Bottom, Height, innerF.Left, innerF.Right, innerF.Bottom, outterF.Bottom);
            verticesBuffer.Put(vertices);
            Quad.Fill(vertices, Right, Width, Bottom, Height, innerF.Right, outterF.Right, innerF.Bottom, outterF.Bottom);
            verticesBuffer.Put(vertices);
        }

        public virtual int MarginLeft()
        {
            return marginLeft;
        }

        public virtual int MarginRight()
        {
            return marginRight;
        }

        public virtual int MarginTop()
        {
            return marginTop;
        }

        public virtual int MarginBottom()
        {
            return marginBottom;
        }

        public virtual int MarginHor()
        {
            return marginLeft + marginRight;
        }

        public virtual int MarginVer()
        {
            return marginTop + marginBottom;
        }

        public virtual float InnerWidth()
        {
            return Width - marginLeft - marginRight;
        }

        public virtual float InnerHeight()
        {
            return Height - marginTop - marginBottom;
        }

        public virtual float InnerRight()
        {
            return Width - marginRight;
        }

        public virtual float InnerBottom()
        {
            return Height - marginBottom;
        }

        public virtual void Size(float Width, float Height)
        {
            _Width = Width;
            _Height = Height;
            UpdateVertices();
        }

        public override void Draw()
        {
            base.Draw();

            var script = NoosaScript.Get();

            texture.Bind();

            script.Camera(Camera);

            script.UModel.valueM4(Matrix);
            script.Lighting(Rm, Gm, Bm, Am, RA, Ga, Ba, Aa);

            script.DrawQuadSet(verticesBuffer, 9);
        }
    }
}