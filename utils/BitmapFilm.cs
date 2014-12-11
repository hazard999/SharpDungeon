using System.Collections.Generic;
using Android.Graphics;

namespace pdsharp.utils
{
	public class BitmapFilm
	{

		public Bitmap bitmap;

		protected internal Dictionary<object, Rect> frames = new Dictionary<object, Rect>();

		public BitmapFilm(Bitmap bitmap)
		{
			this.bitmap = bitmap;
			Add(null, new Rect(0, 0, bitmap.Width, bitmap.Height));
		}

		public BitmapFilm(Bitmap bitmap, int Width) : this(bitmap, Width, bitmap.Height)
		{
		}

		public BitmapFilm(Bitmap bitmap, int Width, int Height)
		{
			this.bitmap = bitmap;
			int cols = bitmap.Width / Width;
			int rows = bitmap.Height / Height;
			for (int i=0; i < rows; i++)
			{
				for (int j=0; j < cols; j++)
				{
					Rect rect = new Rect(j * Width, i * Height, (j+1) * Width, (i+1) * Height);
					Add(i * cols + j, rect);
				}
			}
		}

		public virtual void Add(object id, Rect rect)
		{
			frames.Add(id, rect);
		}

		public virtual Rect Get(object id)
		{
			return frames[id];
		}
	}

}