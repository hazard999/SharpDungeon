using pdsharp.noosa.particles;

namespace sharpdungeon.effects
{
    public class ColdParticle : PixelParticle.Shrinking
    {
        public static Emitter.Factory Factory = new ColdParticleFactory();
     
        public ColdParticle()
        {
            Lifespan = 0.6f;

            Color(0x2244FF);
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan;
            Size(8);
        }

        public override void Update()
        {
            base.Update();

            Am = 1 - Left / Lifespan;
        }
    }
}