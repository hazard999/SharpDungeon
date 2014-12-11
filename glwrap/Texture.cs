using Android.Graphics;
using Android.Opengl;
using Java.Nio;

namespace pdsharp.glwrap
{
    public class Texture
    {
        public const int Nearest = GLES20.GlNearest;
        public const int Linear = GLES20.GlLinear;

        public const int Repeat = GLES20.GlRepeat;
        public const int Mirror = GLES20.GlMirroredRepeat;
        public const int Clamp = GLES20.GlClampToEdge;

        public int Id;

        public bool Premultiplied = false;

        public Texture()
        {
            var ids = new int[1];
            GLES20.GlGenTextures(1, ids, 0);
            Id = ids[0];

            Bind();
        }

        public static void Activate(int index)
        {
            GLES20.GlActiveTexture(GLES20.GlTexture0 + index);
        }

        public virtual void Bind()
        {
            GLES20.GlBindTexture(GLES20.GlTexture2d, Id);
        }

        public virtual void Filter(int minMode, int maxMode)
        {
            Bind();
            GLES20.GlTexParameterf(GLES20.GlTexture2d, GLES20.GlTextureMinFilter, minMode);
            GLES20.GlTexParameterf(GLES20.GlTexture2d, GLES20.GlTextureMagFilter, maxMode);
        }

        public virtual void Wrap(int s, int t)
        {
            Bind();
            GLES20.GlTexParameterf(GLES20.GlTexture2d, GLES20.GlTextureWrapS, s);
            GLES20.GlTexParameterf(GLES20.GlTexture2d, GLES20.GlTextureWrapT, t);
        }

        public virtual void Delete()
        {
            int[] ids = { Id };
            GLES20.GlDeleteTextures(1, ids, 0);
        }

        public virtual void Bitmap(Bitmap bitmap)
        {
            Bind();
            GLUtils.TexImage2D(GLES20.GlTexture2d, 0, bitmap, 0);

            Premultiplied = true;
        }

        public virtual void Pixels(int w, int h, int[] pixels)
        {

            Bind();

            var imageBuffer = ByteBuffer.AllocateDirect(w * h * 4).Order(ByteOrder.NativeOrder()).AsIntBuffer();
            imageBuffer.Put(pixels);
            imageBuffer.Position(0);

            GLES20.GlTexImage2D(GLES20.GlTexture2d, 0, GLES20.GlRgba, w, h, 0, GLES20.GlRgba, GLES20.GlUnsignedByte, imageBuffer);
        }

        public virtual void Pixels(int w, int h, byte[] pixels)
        {
            Bind();

            ByteBuffer imageBuffer = ByteBuffer.AllocateDirect(w * h).Order(ByteOrder.NativeOrder());
            imageBuffer.Put(pixels);
            imageBuffer.Position(0);

            GLES20.GlPixelStorei(GLES20.GlUnpackAlignment, 1);

            GLES20.GlTexImage2D(GLES20.GlTexture2d, 0, GLES20.GlAlpha, w, h, 0, GLES20.GlAlpha, GLES20.GlUnsignedByte, imageBuffer);
        }

        // If getConfig returns null (unsupported format?), GLUtils.texImage2D works
        // incorrectly. In this case we need to load pixels manually
        public virtual void HandMade(Bitmap bitmap, bool recode)
        {
            var w = bitmap.Width;
            var h = bitmap.Height;

            var pixels = new int[w * h];
            bitmap.GetPixels(pixels, 0, w, 0, 0, w, h);

            // recode - components reordering is needed
            if (recode)
            {
                for (var i = 0; i < pixels.Length; i++)
                {
                    var color = pixels[i];
                    var ag = color & Android.Graphics.Color.Argb(0xFF,0x00,0xFF,0x00);
                    var r = (color >> 16) & 0xFF;
                    var b = color & 0xFF;
                    pixels[i] = (int)(ag | (b << 16) | r);
                }
            }

            Pixels(w, h, pixels);

            Premultiplied = false;
        }

        public static Texture Create(Bitmap bmp)
        {
            Texture tex = new Texture();
            tex.Bitmap(bmp);

            return tex;
        }

        public static Texture Create(int Width, int Height, int[] pixels)
        {
            Texture tex = new Texture();
            tex.Pixels(Width, Height, pixels);

            return tex;
        }

        public static Texture Create(int Width, int Height, byte[] pixels)
        {
            var tex = new Texture();
            tex.Pixels(Width, Height, pixels);

            return tex;
        }
    }

}