using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.items.weapon.missiles;
using sharpdungeon.scenes;

namespace sharpdungeon.sprites
{
	public class TenguSprite : MobSprite, ICallback
	{
		private readonly Animation _cast;

		public TenguSprite()
		{
			Texture(Assets.TENGU);

			var frames = new TextureFilm(texture, 14, 16);

			IdleAnimation = new Animation(2, true);
			IdleAnimation.Frames(frames, 0, 0, 0, 1);

			RunAnimation = new Animation(15, false);
			RunAnimation.Frames(frames, 2, 3, 4, 5, 0);

			AttackAnimation = new Animation(15, false);
			AttackAnimation.Frames(frames, 6, 7, 7, 0);

			_cast = AttackAnimation.Clone();

			DieAnimation = new Animation(8, false);
			DieAnimation.Frames(frames, 8, 9, 10, 10, 10, 10, 10, 10);

			Play(RunAnimation.Clone());
		}

		public override void Move(int from, int to)
		{
			Place(to);

			Play(RunAnimation);
			TurnTo(from, to);

			IsMoving = true;

		    if (levels.Level.water[to])
		        GameScene.Ripple(to);

		    Ch.OnMotionComplete();
		}

		public override void Attack(int cell)
		{
		    if (!levels.Level.Adjacent(cell, Ch.pos))
		    {
		        Parent.Recycle<MissileSprite>().Reset(Ch.pos, cell, new Shuriken(), this);

		        Play(_cast);
		        TurnTo(Ch.pos, cell);
		    }
		    else
		        base.Attack(cell);
		}

		public override void OnComplete(Animation anim)
		{
		    if (anim == RunAnimation)
		    {
		        IsMoving = false;
		        Idle();
		    }
		    else
		        base.OnComplete(anim);
		}

	    public void Call()
	    {
            Ch.OnAttackComplete();
	    }
	}
}