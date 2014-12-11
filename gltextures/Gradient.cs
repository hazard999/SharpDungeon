using Android.Graphics;

namespace pdsharp.gltextures
{
    public class Gradient : SmartTexture
    {
        public Gradient(Color[] colors)
            : base(Android.Graphics.Bitmap.CreateBitmap(colors.Length, 1, Android.Graphics.Bitmap.Config.Argb8888))
        {
            for (int i = 0; i < colors.Length; i++)
                bitmap.SetPixel(i, 0, colors[i]);

            Bitmap(bitmap);

            Filter(Linear, Linear);
            Wrap(Clamp, Clamp);

            TextureCache.Add(typeof(Gradient), this);
        }
    }
}