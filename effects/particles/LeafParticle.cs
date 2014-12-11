using pdsharp.noosa.particles;
using pdsharp.utils;

namespace sharpdungeon.effects.particles
{
    public class LeafParticle : PixelParticle.Shrinking
    {
        public static int Color1;
        public static int Color2;

        public static Emitter.Factory Factory = new GeneralLeafParticleFactory();
        public static Emitter.Factory LevelSpecific = new LevelSpecificLeafParticleFactory();
     
        public LeafParticle()
        {
            Lifespan = 1.2f;
            Acc.Set(0, 25);
        }

        public virtual void Reset(float x, float y)
        {
            Revive();

            X = x;
            Y = y;

            Speed.Set(Random.Float(-8, +8), -20);

            Left = Lifespan;
            Size(Random.Float(2, 3));
        }
    }

    public class LevelSpecificLeafParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            var positive = emitter.Recycle<LeafParticle>();
            positive.Color(ColorMath.Random(Dungeon.Level.color1, Dungeon.Level.color2));
            positive.Reset(x, y);
        }
    }

    public class GeneralLeafParticleFactory : Emitter.Factory
    {
        public override void Emit(Emitter emitter, int index, float x, float y)
        {
            var positive = emitter.Recycle<LeafParticle>();
            positive.Color(ColorMath.Random(0x004400, 0x88CC44));
            positive.Reset(x, y);
        }
    }
}