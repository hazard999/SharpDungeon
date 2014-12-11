using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
    public class BurningFistSprite : MobSprite, ICallback
    {
        public BurningFistSprite()
        {
            Texture(Assets.BURNING);

            var frames = new TextureFilm(texture, 24, 17);

            IdleAnimation = new Animation(2, true);
            IdleAnimation.Frames(frames, 0, 0, 1);

            RunAnimation = new Animation(3, true);
            RunAnimation.Frames(frames, 0, 1);

            AttackAnimation = new Animation(8, false);
            AttackAnimation.Frames(frames, 0, 5, 6);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(frames, 0, 2, 3, 4);

            Play(IdleAnimation);
        }

        private int _posToShoot;

        public override void Attack(int cell)
        {
            _posToShoot = cell;
            base.Attack(cell);
        }

        public override void OnComplete(Animation anim)
        {
            if (anim == AttackAnimation)
            {
                Sample.Instance.Play(Assets.SND_ZAP);

                MagicMissile.Shadow(Parent, Ch.pos, _posToShoot, this);

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