using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class WoolParticle : PixelParticle.Shrinking
    {
        public static Emitter.Factory Factory = new WoolParticleFactory();

        public WoolParticle()
        {
            Color(ColorMath.Random(0x999999, 0xEEEEE0));

            Acc.Set(0, -40);
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan = Random.Float(0.6f, 1f);
            Size(5);

            Speed.Set(Random.Float(-10, +10), Random.Float(-10, +10));
        }
    }

    public class WoolParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<WoolParticle>().Reset(x, y);
        }
    }
}