using pdsharp.noosa.particles;

namespace sharpdungeon.effects.particles
{
    public class FlameParticle : PixelParticle.Shrinking
    {
        public static Emitter.Factory Factory = new FlameParticleFactory();

        public FlameParticle()
        {
            Color(0xEE7722);
            Lifespan = 0.6f;

            Acc.Set(0, -80);
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
            Am = p > 0.8f ? (1 - p) * 5 : 1;
        }
    }

    public class FlameParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<FlameParticle>().Reset(x, y);
        }

        public override bool LightMode()
        {
            return true;
        }
    }
}