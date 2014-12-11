using pdsharp.noosa.particles;

namespace sharpdungeon.effects
{
    public class SlowParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<SlowParticle>().Reset(x, y, emitter);
        }

        public override bool LightMode()
        {
            return true;
        }
    }
}