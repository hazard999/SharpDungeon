using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects
{
    public class MagicParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new MagicParticleFactory();

        public MagicParticle()
        {
            Color(0x88CCFF);
            Lifespan = 0.5f;

            Speed.Set(Random.Float(-10, +10), Random.Float(-10, +10));
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan;
        }

        public override void Update()
        {
            base.Update();
            // alpha: 1 -> 0; size: 1 -> 4
            Size(4 - (Am = Left / Lifespan) * 3);
        }
    }
}