using pdsharp.noosa;

namespace sharpdungeon.sprites
{
	public class ShieldedSprite : MobSprite
	{
		public ShieldedSprite()
		{
			Texture(Assets.BRUTE);

			var frames = new TextureFilm(texture, 12, 16);

			IdleAnimation = new Animation(2, true);
			IdleAnimation.Frames(frames, 21, 21, 21, 22, 21, 21, 22, 22);

			RunAnimation = new Animation(12, true);
			RunAnimation.Frames(frames, 25, 26, 27, 28);

			AttackAnimation = new Animation(12, false);
			AttackAnimation.Frames(frames, 23, 24);

			DieAnimation = new Animation(12, false);
			DieAnimation.Frames(frames, 29, 30, 31);

			Play(IdleAnimation);
		}
	}
}