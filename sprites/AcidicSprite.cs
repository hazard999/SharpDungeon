using Android.Graphics;
using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class AcidicSprite : ScorpioSprite
    {
        public AcidicSprite()
        {
            Texture(Assets.SCORPIO);

            var frames = new TextureFilm(texture, 18, 17);

            IdleAnimation = new Animation(12, true);
            IdleAnimation.Frames(frames, 14, 14, 14, 14, 14, 14, 14, 14, 15, 16, 15, 16, 15, 16);

            RunAnimation = new Animation(4, true);
            RunAnimation.Frames(frames, 19, 20);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(frames, 14, 17, 18);

            ZapAnimation = AttackAnimation.Clone();

            DieAnimation = new Animation(12, false);
            DieAnimation.Frames(frames, 14, 21, 22, 23, 24);

            Play(IdleAnimation);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0x66, 0xFF, 0x22);
        }
    }
}