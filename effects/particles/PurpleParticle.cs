using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class PurpleParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new PurpleParticleMissileFactory();

        public static Emitter.Factory Burst = new PurpleParticleBurstFactory();

        public PurpleParticle()
        {
            Lifespan = 0.5f;
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Speed.Set(Random.Float(-5, +5), Random.Float(-5, +5));

            Left = Lifespan;
        }

        public virtual void ResetBurst(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Speed.Polar(Random.Float(360), Random.Float(16, 32));

            Left = Lifespan;
        }

        public override void Update()
        {
            base.Update();
            // alpha: 1 -> 0; size: 1 -> 5
            Size(5 - (Am = Left / Lifespan) * 4);
            // color: 0xFF0044 -> 0x220066
            Color(ColorMath.Interpolate(0x220066, 0xFF0044, Am));
        }
    }

    public class PurpleParticleBurstFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<PurpleParticle>().ResetBurst(x, y);
        }

        public override bool LightMode()
        {
            return true;
        }
    }

    public class PurpleParticleMissileFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<PurpleParticle>().Reset(x, y);
        }
    }
}