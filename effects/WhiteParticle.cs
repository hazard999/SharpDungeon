using pdsharp.noosa.particles;

namespace sharpdungeon.effects
{
    public class WhiteParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new WhiteParticleFactory();
 
        public WhiteParticle()
        {
            Lifespan = 0.4f;

            Am = 0.5f;
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
            // size: 3 -> 0
            Size((Left / Lifespan) * 3);
        }
    }
}