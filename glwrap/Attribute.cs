using Android.Opengl;
using Java.Nio;

namespace pdsharp.glwrap
{
    public class Attribute
    {
        private readonly int _location;

        public Attribute(int location)
        {
            _location = location;
        }

        public virtual int Location()
        {
            return _location;
        }

        public virtual void Enable()
        {
            GLES20.GlEnableVertexAttribArray(_location);
        }

        public virtual void Disable()
        {
            GLES20.GlDisableVertexAttribArray(_location);
        }

        public virtual void VertexPointer(int size, int stride, FloatBuffer ptr)
        {
            //GLES20.GlVertexAttribPointer(_location, size, GLES20.GlFloat, false, stride * sizeof(float) / 8, ptr);
            GLES20.GlVertexAttribPointer(_location, size, GLES20.GlFloat, false, stride * sizeof(float), ptr);
        }
    }

}