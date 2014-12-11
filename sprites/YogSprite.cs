using pdsharp.noosa;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
	public class YogSprite : MobSprite
	{
		public YogSprite()
		{
			Texture(Assets.YOG);

			var frames = new TextureFilm(texture, 20, 19);

			IdleAnimation = new Animation(10, true);
			IdleAnimation.Frames(frames, 0, 1, 2, 2, 1, 0, 3, 4, 4, 3, 0, 5, 6, 6, 5);

			RunAnimation = new Animation(12, true);
			RunAnimation.Frames(frames, 0);

			AttackAnimation = new Animation(12, false);
			AttackAnimation.Frames(frames, 0);

			DieAnimation = new Animation(10, false);
			DieAnimation.Frames(frames, 0, 7, 8, 9);

			Play(IdleAnimation);
		}

		public override void DoDie()
		{
			base.DoDie();

			Splash.At(Center(), Blood(), 12);
		}
	}
}