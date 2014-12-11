using System.Collections.Generic;
using Android.Graphics;

namespace pdsharp.gltextures
{
	public class Atlas
	{
		public SmartTexture tx;

		protected internal Dictionary<object, RectF> namedFrames;

		protected internal float uvLeft;
		protected internal float uvTop;
		protected internal float uvWidth;
		protected internal float uvHeight;
		protected internal int cols;

		public Atlas(SmartTexture tx)
		{

			this.tx = tx;
			tx.Atlas = this;

			namedFrames = new Dictionary<object, RectF>();
		}

		public virtual void Add(object key, int left, int top, int right, int bottom)
		{
			Add(key, UvRect(tx, left, top, right, bottom));
		}

		public virtual void Add(object key, RectF rect)
		{
			namedFrames.Add(key, rect);
		}

		public virtual void grid(int Width)
		{
			grid(Width, tx.Height);
		}

		public virtual void grid(int Width, int Height)
		{
			grid(0, 0, Width, Height, tx.Width / Width);
		}

		public virtual void grid(int left, int top, int Width, int Height, int cols)
		{
			uvLeft = (float)left / tx.Width;
			uvTop = (float)top / tx.Height;
			uvWidth = (float)Width / tx.Width;
			uvHeight= (float)Height / tx.Height;
			this.cols = cols;
		}

		public virtual RectF get(int index)
		{
			float x = index % cols;
			float y = index / cols;
			float l = uvLeft + x * uvWidth;
			float t = uvTop + y * uvHeight;
			return new RectF(l, t, l + uvWidth, t + uvHeight);
		}

		public virtual RectF get(object key)
		{
			return namedFrames[key];
		}

		public virtual float Width(RectF rect)
		{
			return rect.Width() * tx.Width;
		}

		public virtual float Height(RectF rect)
		{
			return rect.Height() * tx.Height;
		}

		public static RectF UvRect(SmartTexture tx, int left, int top, int right, int bottom)
		{
			return new RectF((float)left / tx.Width, (float)top / tx.Height, (float)right / tx.Width, (float)bottom / tx.Height);
		}
	}

}