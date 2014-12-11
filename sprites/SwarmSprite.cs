using Android.Graphics;
using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class SwarmSprite : MobSprite
    {
        public SwarmSprite()
        {
            Texture(Assets.SWARM);

            var frames = new TextureFilm(texture, 16, 16);

            IdleAnimation = new Animation(15, true);
            IdleAnimation.Frames(frames, 0, 1, 2, 3, 4, 5);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(frames, 0, 1, 2, 3, 4, 5);

            AttackAnimation = new Animation(20, false);
            AttackAnimation.Frames(frames, 6, 7, 8, 9);

            DieAnimation = new Animation(15, false);
            DieAnimation.Frames(frames, 10, 11, 12, 13, 14);

            Play(IdleAnimation);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0x8B, 0xA0, 0x77);
        }
    }
}