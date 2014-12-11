using pdsharp.noosa.particles;

namespace sharpdungeon.effects.particles
{
    public class BloodParticle : PixelParticle.Shrinking
    {
        public static Emitter.Factory Factory = new BloodParticleFactory();

        public BloodParticle()
        {
            Color(0xCC0000);
            Lifespan = 0.8f;

            Acc.Set(0, +40);
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan;

            Size(4);
            Speed.Set(0);
        }

        public override void Update()
        {
            base.Update();
            var p = Left / Lifespan;
            Am = p > 0.6f ? (1 - p) * 2.5f : 1;
        }
    }

    public class BloodParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<BloodParticle>().Reset(x, y);
        }
    }
}