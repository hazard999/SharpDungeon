using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class ShadowParticle : PixelParticle.Shrinking
    {
        public static Emitter.Factory Missile = new ShadowParticleMissileFactory();

        public static Emitter.Factory Curse = new ShadowParticleCurseFactory();

        public static Emitter.Factory Up = new ShadowParticleUpFactory();

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Speed.Set(Random.Float(-5, +5), Random.Float(-5, +5));

            Size(6);
            Left = Lifespan = 0.5f;
        }

        public virtual void ResetCurse(float x, float y)
        {
            Revive();

            Size(8);
            Left = Lifespan = 0.5f;

            Speed.Polar(Random.Float(2 * PointF.Pi), Random.Float(16, 32));
            X = x - Speed.X * Lifespan;
            Y = y - Speed.Y * Lifespan;
        }

        public virtual void ResetUp(float x, float y)
        {
            Revive();

            Speed.Set(Random.Float(-8, +8), Random.Float(-32, -48));
            X = x;
            Y = y;

            Size(6);
            Left = Lifespan = 1f;
        }

        public override void Update()
        {
            base.Update();

            var p = Left / Lifespan;
            // alpha: 0 -> 1 -> 0; size: 6 -> 0; color: 0x660044 -> 0x000000
            Color(ColorMath.Interpolate(0x000000, 0x440044, p));
            Am = p < 0.5f ? p * p * 4 : (1 - p) * 2;
        }
    }

    public class ShadowParticleUpFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<ShadowParticle>().ResetUp(x, y);
        }
    }

    public class ShadowParticleCurseFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<ShadowParticle>().ResetCurse(x, y);
        }
    }

    public class ShadowParticleMissileFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<ShadowParticle>().Reset(x, y);
        }
    }
}