using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects
{
    public class SlowParticle : PixelParticle
    {
        private Emitter _emitter;
        
        public static Emitter.Factory Factory = new SlowParticleFactory();
      
        public SlowParticle()
        {

            Lifespan = 0.6f;

            Color(0x664422);
            Size(2);
        }

        public virtual void Reset(float x, float y, Emitter emitter)
        {
            Revive();

            X = x;
            Y = y;
            _emitter = emitter;

            Left = Lifespan;

            Acc.Set(0);
            Speed.Set(Random.Float(-20, +20), Random.Float(-20, +20));
        }

        public override void Update()
        {
            base.Update();

            Am = Left / Lifespan;
            Acc.Set((_emitter.x - X) * 10, (_emitter.y - Y) * 10);
        }
    }
}