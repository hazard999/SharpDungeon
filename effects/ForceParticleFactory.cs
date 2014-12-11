using pdsharp.noosa.particles;

namespace sharpdungeon.effects
{
    public class ForceParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            emitter.Recycle<ForceParticle>().Reset(x, y);
        }
    }
}