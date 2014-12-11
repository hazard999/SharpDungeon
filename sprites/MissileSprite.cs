using System;
using pdsharp.noosa.tweeners;
using pdsharp.utils;
using sharpdungeon.items;

namespace sharpdungeon.sprites
{
	public class MissileSprite : ItemSprite, IListener
	{
		private const float SPEED = 240f;

		private ICallback _callback;

		public MissileSprite()
		{
			OriginToCenter();
		}

		public virtual void Reset(int from, int to, Item item, ICallback listener)
		{
		    if (item == null)
		        Reset(from, to, 0, null, listener);
		    else
		        Reset(from, to, item.image, item.Glowing(), listener);
		}

		public virtual void Reset(int from, int to, int image, Glowing glowing, ICallback listener)
		{
			Revive();

			View(image, glowing);

			_callback = listener;

			Point(DungeonTilemap.TileToWorld(from));
			var dest = DungeonTilemap.TileToWorld(to);

			var d = PointF.Diff(dest, Point());
			Speed.Set(d).Normalize().Scale(SPEED);

		    if (image == 31 || image == 108 || image == 109 || image == 110)
		    {
		        AngularSpeed = 0;
		        Angle = 135 - (float) (Math.Atan2(d.X, d.Y)/3.1415926*180);
		    }
		    else
		        AngularSpeed = image == 15 || image == 106 ? 1440 : 720;

		    var tweener = new PosTweener(this, dest, d.Length / SPEED);
			tweener.Listener = this;
			Parent.Add(tweener);
		}

		public void OnComplete(Tweener tweener)
		{
			Kill();
		    if (_callback != null)
		        _callback.Call();
		}
	}
}