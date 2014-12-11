using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;

namespace sharpdungeon.levels.traps
{
    public class PoisonTrap
    {
        // 0xBB66EE
        public static void Trigger(int pos, Character ch)
        {
            if (ch != null)
                Buff.Affect<Poison>(ch).Set(Poison.DurationFactor(ch) * (4 + Dungeon.Depth / 2));

            CellEmitter.Center(pos).Burst(PoisonParticle.Splash, 3);
        }
    }
}