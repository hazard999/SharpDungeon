
using Android.Opengl;
namespace pdsharp.glwrap
{
    
	public class Uniform
	{

		private int _Location;

		public Uniform(int location)
		{
			_Location = location;
		}

		public virtual int Location()
		{
			return _Location;
		}

		public virtual void enable()
		{
			GLES20.GlEnableVertexAttribArray(_Location);
		}

		public virtual void disable()
		{
			GLES20.GlDisableVertexAttribArray(_Location);
		}

		public virtual void value(int value)
		{
			GLES20.GlUniform1i(_Location, value);
		}

		public virtual void value1f(float value)
		{
			GLES20.GlUniform1f(_Location, value);
		}

		public virtual void value2f(float v1, float v2)
		{
			GLES20.GlUniform2f(_Location, v1, v2);
		}

		public virtual void value4f(float v1, float v2, float v3, float v4)
		{
			GLES20.GlUniform4f(_Location, v1, v2, v3, v4);
		}

		public virtual void valueM3(float[] value)
		{
			GLES20.GlUniformMatrix3fv(_Location, 1, false, value, 0);
		}

		public virtual void valueM4(float[] value)
		{
			GLES20.GlUniformMatrix4fv(_Location, 1, false, value, 0);
		}
	}

}