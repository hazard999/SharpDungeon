using pdsharp.noosa.particles;

namespace sharpdungeon.effects.particles
{
    public class ElmoParticle : PixelParticle.Shrinking
    {
        public static Emitter.Factory Factory = new ElmoParticleFactory();

        public ElmoParticle()
        {
            Color(0x22EE66);
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

    public class ElmoParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<ElmoParticle>().Reset(x, y);
        }

        public override bool LightMode()
        {
            return true;
        }
    }
}