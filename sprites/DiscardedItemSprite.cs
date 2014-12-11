using pdsharp.noosa;

namespace sharpdungeon.sprites
{
	public class DiscardedItemSprite : ItemSprite
	{
		public DiscardedItemSprite()
		{
			OriginToCenter();
			AngularSpeed = 720;
		}

		public override void Drop()
		{
			Scale.Set(1);
			Am = 1;
		}

		public override void Update()
		{
			base.Update();

			Scale.Set(Scale.X * 0.9f);
		    if ((Am -= Game.Elapsed) <= 0)
		        Remove();
		}
	}
}