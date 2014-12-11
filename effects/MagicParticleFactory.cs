using pdsharp.noosa.particles;

namespace sharpdungeon.effects
{
    public class MagicParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<MagicParticle>().Reset(x, y);
        }
        public override bool LightMode()
        {
            return true;
        }
    }
}