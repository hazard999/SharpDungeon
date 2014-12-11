using Android.Graphics;
using pdsharp.noosa;
using sharpdungeon.effects;

namespace sharpdungeon.sprites
{
    public class DM300Sprite : MobSprite
    {
        public DM300Sprite()
        {
            Texture(Assets.DM300);

            var frames = new TextureFilm(texture, 22, 20);

            IdleAnimation = new Animation(10, true);
            IdleAnimation.Frames(frames, 0, 1);

            RunAnimation = new Animation(10, true);
            RunAnimation.Frames(frames, 2, 3);

            AttackAnimation = new Animation(15, false);
            AttackAnimation.Frames(frames, 4, 5, 6);

            DieAnimation = new Animation(20, false);
            DieAnimation.Frames(frames, 0, 7, 0, 7, 0, 7, 0, 7, 0, 7, 0, 7, 8);

            Play(IdleAnimation);
        }

        public override void OnComplete(Animation anim)
        {
            base.OnComplete(anim);

            if (anim == DieAnimation)
                Emitter().Burst(Speck.Factory(Speck.WOOL), 15);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0xFF, 0xFF, 0x88);
        }
    }
}