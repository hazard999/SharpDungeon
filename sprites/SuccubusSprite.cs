using pdsharp.noosa;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;

namespace sharpdungeon.sprites
{
    public class SuccubusSprite : MobSprite
    {
        public SuccubusSprite()
        {
            Texture(Assets.SUCCUBUS);

            var frames = new TextureFilm(texture, 12, 15);

            IdleAnimation = new Animation(8, true);
            IdleAnimation.Frames(frames, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 2, 2, 2, 1);

            RunAnimation = new Animation(15, true);
            RunAnimation.Frames(frames, 3, 4, 5, 6, 7, 8);

            AttackAnimation = new Animation(12, false);
            AttackAnimation.Frames(frames, 9, 10, 11);

            DieAnimation = new Animation(10, false);
            DieAnimation.Frames(frames, 12);

            Play(IdleAnimation);
        }

        public override void DoDie()
        {
            base.DoDie();
            Emitter().Burst(Speck.Factory(Speck.HEART), 6);
            Emitter().Burst(ShadowParticle.Up, 8);
        }
    }
}