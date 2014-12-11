using Android.Graphics;
using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class SpinnerSprite : MobSprite
    {
        public SpinnerSprite()
        {
            Texture(Assets.SPINNER);

            var frames = new TextureFilm(texture, 16, 16);

            IdleAnimation = new Animation(10, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 0, 0, 1, 0, 1);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(frames, 0, 2, 0, 3);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(frames, 0, 4, 5, 0);

            DieAnimation = new Animation(12, false);
            DieAnimation.Frames(frames, 6, 7, 8, 9);

            Play(IdleAnimation);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0xBF, 0xE5, 0xB8);
        }
    }
}