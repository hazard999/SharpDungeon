using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class RottingFistSprite : MobSprite
    {
        private const float FallSpeed = 64;

        public RottingFistSprite()
        {
            Texture(Assets.ROTTING);

            var frames = new TextureFilm(texture, 24, 17);

            IdleAnimation = new Animation(2, true);
            IdleAnimation.Frames(frames, 0, 0, 1);

            RunAnimation = new Animation(3, true);
            RunAnimation.Frames(frames, 0, 1);

            AttackAnimation = new Animation(2, false);
            AttackAnimation.Frames(frames, 0);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(frames, 0, 2, 3, 4);

            Play(IdleAnimation);
        }

        public override void Attack(int cell)
        {
            base.Attack(cell);

            Speed.Set(0, -FallSpeed);
            Acc.Set(0, FallSpeed * 4);
        }

        public override void OnComplete(Animation anim)
        {
            base.OnComplete(anim);

            if (anim != AttackAnimation)
                return;

            Speed.Set(0);
            Acc.Set(0);
            Place(Ch.pos);

            Camera.Main.Shake(4, 0.2f);
        }
    }
}