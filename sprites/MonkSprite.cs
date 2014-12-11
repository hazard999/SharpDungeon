using pdsharp.noosa;
using pdsharp.utils;

namespace sharpdungeon.sprites
{
	public class MonkSprite : MobSprite
	{
        private readonly Animation _kick;

		public MonkSprite()
		{
			Texture(Assets.MONK);

			var frames = new TextureFilm(texture, 15, 14);

			IdleAnimation = new Animation(6, true);
			IdleAnimation.Frames(frames, 1, 0, 1, 2);

			RunAnimation = new Animation(15, true);
			RunAnimation.Frames(frames, 11, 12, 13, 14, 15, 16);

			AttackAnimation = new Animation(12, false);
			AttackAnimation.Frames(frames, 3, 4, 3, 4);

			_kick = new Animation(10, false);
			_kick.Frames(frames, 5, 6, 5);

			DieAnimation = new Animation(15, false);
			DieAnimation.Frames(frames, 1, 7, 8, 8, 9, 10);

			Play(IdleAnimation);
		}

		public override void Attack(int cell)
		{
			base.Attack(cell);
		    if (Random.Float() < 0.5f)
		        Play(_kick);
		}

		public override void OnComplete(Animation anim)
		{
			base.OnComplete(anim == _kick ? AttackAnimation : anim);
		}
	}
}