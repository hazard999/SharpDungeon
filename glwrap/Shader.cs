using Android.Opengl;
using System;
namespace pdsharp.glwrap
{

	public class Shader
	{

		public const int VERTEX = GLES20.GlVertexShader;
		public const int FRAGMENT = GLES20.GlFragmentShader;

		private int handle;

		public Shader(int type)
		{
			handle = GLES20.GlCreateShader(type);
		}

		public virtual int Handle()
		{
			return handle;
		}

		public virtual void Source(string src)
		{
			GLES20.GlShaderSource(handle, src);
		}

		public virtual void Compile()
		{
			GLES20.GlCompileShader(handle);

			int[] status = new int[1];
			GLES20.GlGetShaderiv(handle, GLES20.GlCompileStatus, status, 0);
			if (status[0] == GLES20.GlFalse)
			{
				throw new Exception(GLES20.GlGetShaderInfoLog(handle));
			}
		}

		public virtual void Delete()
		{
			GLES20.GlDeleteShader(handle);
		}

		public static Shader CreateCompiled(int type, string src)
		{
			Shader shader = new Shader(type);
			shader.Source(src);
			shader.Compile();
			return shader;
		}
	}

}