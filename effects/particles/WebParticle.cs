using pdsharp.noosa.particles;

namespace sharpdungeon.effects.particles
{
    public class WebParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new WebParticleFactory();
     
        public WebParticle()
        {
            Color(0xCCCCCC);
            Lifespan = 2f;
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Left = Lifespan;
            Angle = pdsharp.utils.Random.Float(360);
        }

        public override void Update()
        {
            base.Update();

            var p = Left / Lifespan;
            Am = p < 0.5f ? p : 1 - p;
            Scale.Y = 16 + p * 8;
        }
    }

    public class WebParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            for (var information = 0; information < 3; information++)
                emitter.Recycle<WebParticle>().Reset(x, y);
        }
    }
}