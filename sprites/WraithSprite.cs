using Android.Graphics;
using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class WraithSprite : MobSprite
    {
        public WraithSprite()
        {
            Texture(Assets.WRAITH);

            var frames = new TextureFilm(texture, 14, 15);

            IdleAnimation = new Animation(5, true);
            IdleAnimation.Frames(frames, 0, 1);

            RunAnimation = new Animation(10, true);
            RunAnimation.Frames(frames, 0, 1);

            AttackAnimation = new Animation(10, false);
            AttackAnimation.Frames(frames, 0, 2, 3);

            DieAnimation = new Animation(8, false);
            DieAnimation.Frames(frames, 0, 4, 5, 6, 7);

            Play(IdleAnimation);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0x88, 0x00, 0x00, 0x00);
        }
    }
}