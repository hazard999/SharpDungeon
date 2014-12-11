using Android.Graphics;
using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class CrabSprite : MobSprite
    {
        public CrabSprite()
        {
            Texture(Assets.CRAB);

            var frames = new TextureFilm(texture, 16);

            IdleAnimation = new Animation(5, true);
            IdleAnimation.Frames(frames, 0, 1, 0, 2);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(frames, 3, 4, 5, 6);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(frames, 7, 8, 9);

            DieAnimation = new Animation(12, false);
            DieAnimation.Frames(frames, 10, 11, 12, 13);

            Play(IdleAnimation);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0xFF, 0xEA, 0x80);
        }
    }
}