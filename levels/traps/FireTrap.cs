using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.scenes;

namespace sharpdungeon.levels.traps
{
    public class FireTrap
    {
        // 0xFF7708
        public static void Trigger(int pos, Character ch)
        {
            GameScene.Add(Blob.Seed(pos, 2, typeof(Fire)));
            CellEmitter.Get(pos).Burst(FlameParticle.Factory, 5);
        }
    }
}