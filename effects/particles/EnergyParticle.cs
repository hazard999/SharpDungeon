using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class EnergyParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new EnergyParticleFactory();

        public EnergyParticle()
        {

            Lifespan = 1f;
            Color(0xFFFFAA);

            Speed.Polar(Random.Float(2 * PointF.Pi), Random.Float(24, 32));
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            Left = Lifespan;

            X = x - Speed.X * Lifespan;
            Y = y - Speed.Y * Lifespan;
        }

        public override void Update()
        {
            base.Update();

            var p = Left / Lifespan;
            Am = p < 0.5f ? p * p * 4 : (1 - p) * 2;
            Size(Random.Float(5 * Left / Lifespan));
        }
    }

    public class EnergyParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<EnergyParticle>().Reset(x, y);
        }

        public override bool LightMode()
        {
            return true;
        }
    }
}