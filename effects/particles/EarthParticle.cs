using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class EarthParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new EarthParticleFactory();

        public EarthParticle()
        {
            Color(ColorMath.Random(0x444444, 0x777766));
            Angle = Random.Float(-30, 30);

            Lifespan = 0.5f;
        }

        public virtual void reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan;
        }

        public override void Update()
        {
            base.Update();

            var p = Left / Lifespan;
            Size((p < 0.5f ? p : 1 - p) * 16);
        }
    }

    public class EarthParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
           emitter.Recycle<EarthParticle>().reset(x, y);
        }
    }
}