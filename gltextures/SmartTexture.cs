using Android.Graphics;
using pdsharp.glwrap;

namespace pdsharp.gltextures
{    
	public class SmartTexture : Texture
	{
	    public int FModeMin;
		public int FModeMax;

		public int WModeH;
		public int WModeV;

	    public Bitmap bitmap;

		public Atlas Atlas;

		public SmartTexture(Bitmap bitmap) : this(bitmap, Nearest, Clamp)
		{
		}

		public SmartTexture(Bitmap bitmap, int filtering, int wrapping)
		{
            Bitmap(bitmap);
			Filter(filtering, filtering);
			Wrap(wrapping, wrapping);
		}

	    public int Width { get; set; }

	    public int Height { get; set; }

	    public override void Filter(int minMode, int maxMode)
		{
			base.Filter(FModeMin = minMode, FModeMax = maxMode);
		}

		public override void Wrap(int s, int t)
		{
			base.Wrap(WModeH = s, WModeV = t);
		}

		public override void Bitmap(Bitmap bitmap)
		{
			Bitmap(bitmap, false);
		}

		public virtual void Bitmap(Bitmap bitmap, bool premultiplied)
		{
		    if (premultiplied)
		        base.Bitmap(bitmap);
		    else
		        HandMade(bitmap, true);

		    this.bitmap = bitmap;
			Width = bitmap.Width;
			Height = bitmap.Height;
		}

		public virtual void Reload()
		{
			Id = new SmartTexture(bitmap).Id;
			Filter(FModeMin, FModeMax);
			Wrap(WModeH, WModeV);
		}

		public override void Delete()
		{
			base.Delete();

			bitmap.Recycle();
			bitmap = null;
		}

		public virtual RectF UvRect(int left, int top, int right, int bottom)
		{
			return new RectF((float)left / Width, (float)top / Height, (float)right / Width, (float)bottom / Height);
		}
	}
}