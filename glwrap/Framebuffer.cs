using Android.Opengl;

namespace pdsharp.glwrap
{
	public class Framebuffer
	{
		public const int COLOR = GLES20.GlColorAttachment0;
		public const int DEPTH = GLES20.GlDepthAttachment;
		public const int STENCIL = GLES20.GlStencilAttachment;

		public static readonly Framebuffer system = new Framebuffer(0);

		private int id;

		public Framebuffer()
		{
			int[] buffers = new int[1];
			GLES20.GlGenBuffers(1, buffers, 0);
			id = buffers[0];
		}

		private Framebuffer(int n)
		{

		}

		public virtual void bind()
		{
			GLES20.GlBindFramebuffer(GLES20.GlFramebuffer, id);
		}

		public virtual void delete()
		{
			int[] buffers = {id};
			GLES20.GlDeleteFramebuffers(1, buffers, 0);
		}

		public virtual void attach(int point, Texture tex)
		{
			bind();
            GLES20.GlFramebufferTexture2D(GLES20.GlFramebuffer, point, GLES20.GlTexture2d, tex.Id, 0);
		}

		public virtual void attach(int point, Renderbuffer buffer)
		{
			bind();
            GLES20.GlFramebufferRenderbuffer(GLES20.GlRenderbuffer, point, GLES20.GlTexture2d, buffer.Id());
		}

		public virtual bool status()
		{
			bind();
            return GLES20.GlCheckFramebufferStatus(GLES20.GlFramebuffer) == GLES20.GlFramebufferComplete;
		}
	}

}