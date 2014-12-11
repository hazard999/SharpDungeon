using Android.Opengl;
namespace pdsharp.glwrap
{   
    public class Renderbuffer
    {
        public const int RGBA8 = GLES20.GlRgba; // ?
        public const int DEPTH16 = GLES20.GlDepthComponent;
        public const int STENCIL8 = GLES20.GlStencilIndex8;

        private int id;

        public Renderbuffer()
        {
            int[] buffers = new int[1];
            GLES20.GlGenRenderbuffers(1, buffers, 0);
            id = buffers[0];
        }

        public virtual int Id()
        {
            return id;
        }

        public virtual void Bind()
        {
            GLES20.GlBindRenderbuffer(GLES20.GlRenderbuffer, id);
        }

        public virtual void Delete()
        {
            int[] buffers = {id};
            GLES20.GlDeleteRenderbuffers(1, buffers, 0);
        }

        public virtual void Storage(int format, int Width, int Height)
        {
            GLES20.GlRenderbufferStorage(GLES20.GlRenderbuffer, format, Width, Height);
        }
    }

}