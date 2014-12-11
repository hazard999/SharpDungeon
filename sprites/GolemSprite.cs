using Android.Graphics;
using pdsharp.noosa;
using sharpdungeon.effects.particles;

namespace sharpdungeon.sprites
{
    public class GolemSprite : MobSprite
    {
        public GolemSprite()
        {
            Texture(Assets.GOLEM);

            var frames = new TextureFilm(texture, 16, 16);

            IdleAnimation = new Animation(4, true);
            IdleAnimation.Frames(frames, 0, 1);

            RunAnimation = new Animation(12, true);
            RunAnimation.Frames(frames, 2, 3, 4, 5);

            AttackAnimation = new Animation(10, false);
            AttackAnimation.Frames(frames, 6, 7, 8);

            DieAnimation = new Animation(15, false);
            DieAnimation.Frames(frames, 9, 10, 11, 12, 13);

            Play(IdleAnimation);
        }

        public override Color Blood()
        {
            return Android.Graphics.Color.Argb(0xFF, 0x80, 0x70, 0x6c);
        }

        public override void OnComplete(Animation anim)
        {
            if (anim == DieAnimation)
                Emitter().Burst(ElmoParticle.Factory, 4);

            base.OnComplete(anim);
        }
    }
}