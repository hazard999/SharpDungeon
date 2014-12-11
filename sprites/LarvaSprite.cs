using Android.Graphics;
using pdsharp.noosa;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
    public class LarvaSprite : MobSprite
    {
        public LarvaSprite()
        {
            Texture(Assets.LARVA);

            var frames = new TextureFilm(texture, 12, 8);

            IdleAnimation = new Animation(5, true);
            IdleAnimation.Frames(frames, 4, 4, 4, 4, 4, 5, 5);

            RunAnimation = new Animation(12, true);
            RunAnimation.Frames(frames, 0, 1, 2, 3);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(frames, 6, 5, 7);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(frames, 8);

            Play(IdleAnimation);
        }

        public override Color Blood()
        {
            return new Color(0xbbcc66);
        }

        public override void DoDie()
        {
            Splash.At(Center(), Blood(), 10);
            base.DoDie();
        }
    }
}