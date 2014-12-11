using Android.Graphics;
using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class StatueSprite : MobSprite
    {
        public StatueSprite()
        {
            Texture(Assets.STATUE);

            var frames = new TextureFilm(texture, 12, 15);

            IdleAnimation = new Animation(2, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 0, 0, 1, 1);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(frames, 2, 3, 4, 5, 6, 7);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(frames, 8, 9, 10);

            DieAnimation = new Animation(5, false);
            DieAnimation.Frames(frames, 11, 12, 13, 14, 15, 15);

            Play(IdleAnimation);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0xcd, 0xcd, 0xb7);
        }
    }
}