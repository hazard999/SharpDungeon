using System;
using Android.Opengl;

namespace pdsharp.glwrap
{
	public class Program
	{
		private readonly int _handle;

		public Program()
		{
			_handle = GLES20.GlCreateProgram();
		}

		public virtual int Handle()
		{
			return _handle;
		}

		public virtual void Attach(Shader shader)
		{
			GLES20.GlAttachShader(_handle, shader.Handle());
		}

		public virtual void Link()
		{
			GLES20.GlLinkProgram(_handle);

			int[] status = new int[1];
			GLES20.GlGetProgramiv(_handle, GLES20.GlLinkStatus, status, 0);
			if (status[0] == GLES20.GlFalse)
			{
				throw new Exception(GLES20.GlGetProgramInfoLog(_handle));
			}
		}

		public virtual Attribute Attribute(string name)
		{
			return new Attribute(GLES20.GlGetAttribLocation(_handle, name));
		}

		public virtual Uniform Uniform(string name)
		{
			return new Uniform(GLES20.GlGetUniformLocation(_handle, name));
		}

		public virtual void Use()
		{
			GLES20.GlUseProgram(_handle);
		}

		public virtual void Delete()
		{
			GLES20.GlDeleteProgram(_handle);
		}

		public static Program Create(params Shader[] shaders)
		{
			var program = new Program();
		    
            foreach (var shader in shaders)
		        program.Attach(shader);
            
            program.Link();
			return program;
		}
	}

}