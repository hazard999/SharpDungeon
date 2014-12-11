using Android.Graphics;

namespace pdsharp.noosa
{
    public class SkinnedBlock : Image
    {

        protected internal float scaleX;
        protected internal float scaleY;

        protected internal float offsetX;
        protected internal float offsetY;

        public SkinnedBlock(float Width, float Height, object tx)
            : base(tx)
        {

            texture.Wrap(pdsharp.glwrap.Texture.Repeat, pdsharp.glwrap.Texture.Repeat);

            Size(Width, Height);
        }

        public override void Frame(RectF frame)
        {
            scaleX = 1;
            scaleY = 1;

            offsetX = 0;
            offsetY = 0;

            base.Frame(new RectF(0, 0, 1, 1));
        }

        protected internal override void UpdateFrame()
        {

            float tw = 1f / texture.Width;
            float th = 1f / texture.Height;

            float u0 = offsetX * tw;
            float v0 = offsetY * th;
            float u1 = u0 + Width * tw / scaleX;
            float v1 = v0 + Height * th / scaleY;

            vertices[2] = u0;
            vertices[3] = v0;

            vertices[6] = u1;
            vertices[7] = v0;

            vertices[10] = u1;
            vertices[11] = v1;

            vertices[14] = u0;
            vertices[15] = v1;

            dirty = true;
        }

        public virtual void OffsetTo(float x, float y)
        {
            offsetX = x;
            offsetY = y;
            UpdateFrame();
        }

        public virtual void Offset(float x, float y)
        {
            offsetX += x;
            offsetY += y;
            UpdateFrame();
        }

        public virtual float OffsetX()
        {
            return offsetX;
        }

        public virtual float OffsetY()
        {
            return offsetY;
        }

        public virtual void scale(float x, float y)
        {
            scaleX = x;
            scaleY = y;
            UpdateFrame();
        }

        public virtual void Size(float w, float h)
        {
            _Width = w;
            _Height = h;
            UpdateFrame();
            updateVertices();
        }
    }
}