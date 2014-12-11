using pdsharp.noosa.particles;
using pdsharp.utils;
using sharpdungeon.effects.particles;

namespace sharpdungeon.effects
{
    public class EarthParticle : PixelParticle.Shrinking
    {
        public static Emitter.Factory Factory = new EarthParticleFactory();
       
        public EarthParticle()
        {
            Lifespan = 0.5f;

            Color(ColorMath.Random(0x555555, 0x777766));

            Acc.Set(0, +40);
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            X = y;

            Left = Lifespan;
            Size(4);

            Speed.Set(Random.Float(-10, +10), Random.Float(-10, +10));
        }
    }
}