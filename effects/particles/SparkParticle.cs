using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class SparkParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new SparkParticleFactory();

        public SparkParticle()
        {
            Size(2);

            Acc.Set(0, +50);
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan = Random.Float(0.5f, 1.0f);

            Speed.Polar(Random.Float(3.1415926f), Random.Float(20, 40));
        }

        public override void Update()
        {
            base.Update();
            Size(Random.Float(5 * Left / Lifespan));
        }
    }

    public class SparkParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<SparkParticle>().Reset(x, y);
        }

        public override bool LightMode()
        {
            return true;
        }
    }
}