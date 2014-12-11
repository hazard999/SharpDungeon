using pdsharp.noosa.particles;

namespace sharpdungeon.effects.particles
{
    public class ShaftParticle : PixelParticle
    {
        public static Emitter.Factory Factory = new ShaftParticleFactory();

        public ShaftParticle()
        {
            Lifespan = 1.2f;
            Speed.Set(0, -6);
        }

        private float _offs;

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            _offs = -pdsharp.utils.Random.Float(Lifespan);
            Left = Lifespan - _offs;
        }

        public override void Update()
        {
            base.Update();

            var p = Left / Lifespan;
            Am = p < 0.5f ? p : 1 - p;
            Scale.X = (1 - p) * 4;
            Scale.Y = 16 + (1 - p) * 16;
        }
    }

    public class ShaftParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<ShaftParticle>().Reset(x, y);
        }

        public override bool LightMode()
        {
            return true;
        }
    }
}