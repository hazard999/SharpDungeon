using Android.Graphics;
using pdsharp.gltextures;
using pdsharp.noosa;

namespace sharpdungeon.effects
{
	public class Halo : Image
	{
		private static readonly object CacheKey = typeof(Halo);

		protected internal const int RADIUS = 64;

		protected internal float radius = RADIUS;
		protected internal float brightness = 1;

		public Halo()
		{
			if (!TextureCache.Contains(CacheKey))
			{
				var bmp = Bitmap.CreateBitmap(RADIUS * 2, RADIUS * 2, Bitmap.Config.Argb8888);
				var canvas = new Canvas(bmp);
				var paint = new Paint();

                paint.Color = Android.Graphics.Color.Argb(0xFF,0xFF,0xFF,0xFF);
				canvas.DrawCircle(RADIUS, RADIUS, RADIUS * 0.75f, paint);
				paint.Color = Android.Graphics.Color.Argb(0x88,0xFF,0xFF,0xFF);
				canvas.DrawCircle(RADIUS, RADIUS, RADIUS, paint);
				TextureCache.Add(CacheKey, new SmartTexture(bmp));
			}

			Texture(CacheKey);

			Origin.Set(RADIUS);
		}

		public Halo(float radius, int color, float brightness) : this()
		{
			Hardlight(color);
			Alpha(this.brightness = brightness);
			Radius(radius);
		}

		public virtual Halo Point(float x, float y)
		{
			X = x - RADIUS;
			Y = y - RADIUS;
			return this;
		}

		public virtual void Radius(float value)
		{
			Scale.Set((radius = value) / RADIUS);
		}
	}
}