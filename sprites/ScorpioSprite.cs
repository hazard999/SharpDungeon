using Android.Graphics;
using pdsharp.noosa;

namespace sharpdungeon.sprites
{
	public class ScorpioSprite : MobSprite
	{
		private int cellToAttack;

		public ScorpioSprite() : base()
		{
			Texture(Assets.SCORPIO);

			var frames = new TextureFilm(texture, 18, 17);

			IdleAnimation = new Animation(12, true);
			IdleAnimation.Frames(frames, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 1, 2, 1, 2);

			RunAnimation = new Animation(8, true);
			RunAnimation.Frames(frames, 5, 5, 6, 6);

			AttackAnimation = new Animation(15, false);
			AttackAnimation.Frames(frames, 0, 3, 4);

			ZapAnimation = AttackAnimation.Clone();

			DieAnimation = new Animation(12, false);
			DieAnimation.Frames(frames, 0, 7, 8, 9, 10);

			Play(IdleAnimation);
		}

		public override Color Blood()
		{
			return Android.Graphics.Color.Argb( 0xFF,0x44,0xFF,0x22);
		}

		public override void Attack(int cell)
		{
		    if (!levels.Level.Adjacent(cell, Ch.pos))
		    {
		        cellToAttack = cell;
		        TurnTo(Ch.pos, cell);
		        Play(ZapAnimation);
		    }
		    else 
                base.Attack(cell);
		}

		public override void OnComplete(Animation anim)
		{
		    if (anim == ZapAnimation)
		    {
		        Idle();
                
                //FIX ICallback
		        //((MissileSprite)Parent.Recycle(typeof(MissileSprite))).Reset(Ch.pos, cellToAttack, new Dart(), new ICallback() { public void call() { ch.onAttackComplete(); } });
		    }
		    else
		        base.OnComplete(anim);
		}
	}
}