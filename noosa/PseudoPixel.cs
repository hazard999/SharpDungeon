using pdsharp.gltextures;

namespace pdsharp.noosa
{
    public class PseudoPixel : Image
    {
        public PseudoPixel()
            : base(TextureCache.CreateSolid(Android.Graphics.Color.Argb(255, 255, 255, 255)))
        {

        }

        public PseudoPixel(float x, float y, int color)
            : this()
        {
            X = x;
            Y = y;
            Color(color);
        }

        public virtual void Size(float w, float h)
        {
            Scale.Set(w, h);
        }

        public virtual void Size(float value)
        {
            Scale.Set(value);
        }
    }
}