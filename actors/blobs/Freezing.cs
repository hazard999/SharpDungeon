using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;

namespace sharpdungeon.actors.blobs
{
    public class Freezing
    {
        public static void Affect(int cell, Fire fire)
        {
            var ch = Actor.FindChar(cell);
            if (ch != null)
                Buff.Prolong<Frost>(ch, Frost.duration(ch) * pdsharp.utils.Random.Float(1.0f, 1.5f));

            if (fire != null)
                fire.Clear(cell);

            var heap = Dungeon.Level.heaps[cell];
            if (heap != null)
                heap.Freeze();

            if (Dungeon.Visible[cell])
                CellEmitter.Get(cell).Start(SnowParticle.Factory, 0.2f, 6);
        }
    }
}