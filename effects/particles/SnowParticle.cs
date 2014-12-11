using pdsharp.noosa.particles;

namespace sharpdungeon.effects.particles
{
    public class SnowParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new SnowParticleFactory();

        public SnowParticle()
        {
            Speed.Set(0, pdsharp.utils.Random.Float(5, 8));
            Lifespan = 1.2f;
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y - Speed.Y * Lifespan;

            Left = Lifespan;
        }

        public override void Update()
        {
            base.Update();
            var p = Left / Lifespan;
            Am = (p < 0.5f ? p : 1 - p) * 1.5f;
        }
    }

    public class SnowParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<SnowParticle>().Reset(x, y);
        }
    }
}