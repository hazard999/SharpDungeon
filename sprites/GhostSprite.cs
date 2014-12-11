using Android.Graphics;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using pdsharp.noosa;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;

namespace sharpdungeon.sprites
{
    public class GhostSprite : MobSprite
    {
        public GhostSprite()
        {
            Texture(Assets.GHOST);

            TextureFilm frames = new TextureFilm(texture, 14, 15);

            IdleAnimation = new Animation(5, true);
            IdleAnimation.Frames(frames, 0, 1);

            RunAnimation = new Animation(10, true);
            RunAnimation.Frames(frames, 0, 1);

            DieAnimation = new Animation(20, false);
            DieAnimation.Frames(frames, 0);

            Play(IdleAnimation);
        }

        public override void Draw()
        {
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOne);
            base.Draw();
            GLES20.GlBlendFunc(GL10.GlSrcAlpha, GL10.GlOneMinusSrcAlpha);
        }

        public override void DoDie()
        {
            base.DoDie();
            Emitter().Start(ShaftParticle.Factory, 0.3f, 4);
            Emitter().Start(Speck.Factory(Speck.LIGHT), 0.2f, 3);
        }

        public override Color Blood()
        {
            return new Color(0xFFFFFF);
        }
    }
}