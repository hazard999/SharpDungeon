using Android.Graphics;
using pdsharp.gltextures;

namespace pdsharp.noosa
{
	public class ColorBlock : Image, Resizable
	{
		public ColorBlock(float Width, float Height, Color color) : base(TextureCache.CreateSolid(color))
		{
			Scale.Set(Width, Height);
			Origin.Set(0, 0);
		}

		public void Size(float Width, float Height)
		{
			Scale.Set(Width, Height);
		}

		public override float Width
		{
		    get { return Scale.X; }
		}

		public override float Height
		{
		    get { return Scale.Y; }
		}
	}

}