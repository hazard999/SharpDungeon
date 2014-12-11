using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects
{
    public class ForceParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new ForceParticleFactory();

        public ForceParticle()
        {
            Lifespan = 0.6f;

            Size(4);
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan;

            Acc.Set(0);
            Speed.Set(Random.Float(-40, +40), Random.Float(-40, +40));
        }

        public override void Update()
        {
            base.Update();

            Am = (Left / Lifespan) / 2;
            Acc.Set(-Speed.X * 10, -Speed.Y * 10);
        }
    }
}