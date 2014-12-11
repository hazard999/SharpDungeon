using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
    public class WarlockSprite : MobSprite
	{
		public WarlockSprite() : base()
		{
			Texture(Assets.WARLOCK);

			var frames = new TextureFilm(texture, 12, 15);

			IdleAnimation = new Animation(2, true);
			IdleAnimation.Frames(frames, 0, 0, 0, 1, 0, 0, 1, 1);

			RunAnimation = new Animation(15, true);
			RunAnimation.Frames(frames, 0, 2, 3, 4);

			AttackAnimation = new Animation(12, false);
			AttackAnimation.Frames(frames, 0, 5, 6);

			ZapAnimation = AttackAnimation.Clone();

			DieAnimation = new Animation(15, false);
			DieAnimation.Frames(frames, 0, 7, 8, 8, 9, 10);

			Play(IdleAnimation);
		}

		public void Zap(int cell)
		{

			TurnTo(Ch.pos, cell);
			Play(ZapAnimation);

            //MagicMissile.Shadow(parent, Ch.pos, cell, new ICallback() { public void call() { ((Warlock)ch).onZapComplete(); } });
			Sample.Instance.Play(Assets.SND_ZAP);
		}

		public override void OnComplete(Animation anim)
		{
		    if (anim == ZapAnimation)
		        Idle();
		    
            base.OnComplete(anim);
		}
	}
}