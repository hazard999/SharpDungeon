using Android.Graphics;
using pdsharp.noosa;

namespace sharpdungeon.sprites
{
    public class GooSprite : MobSprite
    {
        private readonly Animation _pumpAnimation;

        public GooSprite()
        {
            Texture(Assets.GOO);

            var frames = new TextureFilm(texture, 20, 14);

            IdleAnimation = new Animation(10, true);
            IdleAnimation.Frames(frames, 0, 1);

            RunAnimation = new Animation(10, true);
            RunAnimation.Frames(frames, 0, 1);

            _pumpAnimation = new Animation(20, true);
            _pumpAnimation.Frames(frames, 0, 1);

            AttackAnimation = new Animation(10, false);
            AttackAnimation.Frames(frames, 5, 0, 6);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(frames, 2, 3, 4);

            Play(IdleAnimation);
        }

        public virtual void PumpUp()
        {
            Play(_pumpAnimation);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0x00, 0x00, 0x00);
        }
    }
}