using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class MisslePoisonParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<PoisonParticle>().ResetMissile(x, y);
        }

        public override bool LightMode()
        {
            return true;
        }
    }

    public class SplashPoisonParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<PoisonParticle>().ResetSplash(x, y);
        }

        public override bool LightMode()
        {
            return true;
        }
    }

    public class PoisonParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new MisslePoisonParticleFactory();
        
        public static Emitter.Factory Splash = new SplashPoisonParticleFactory();
        
        public PoisonParticle()
        {
            Lifespan = 0.6f;

            Acc.Set(0, +30);
        }

        public virtual void ResetMissile(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan;

            Speed.Polar(Random.Float(3.1415926f), Random.Float(6));
        }

        public virtual void ResetSplash(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan;

            Speed.Polar(Random.Float(3.1415926f), Random.Float(10, 20));
        }

        public override void Update()
        {
            base.Update();
            // alpha: 1 -> 0; size: 1 -> 4
            Size(4 - (Am = Left / Lifespan) * 3);
            // color: 0x8844FF -> 0x00FF00
            Color(ColorMath.Interpolate(0x00FF00, 0x8844FF, Am));
        }
    }
}