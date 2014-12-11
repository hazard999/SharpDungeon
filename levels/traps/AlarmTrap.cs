using System.Linq;
using pdsharp.noosa.audio;
using sharpdungeon.actors;
using sharpdungeon.effects;
using sharpdungeon.utils;

namespace sharpdungeon.levels.traps
{
    public class AlarmTrap
    {
        // 0xDD3333
        public static void Trigger(int pos, Character ch)
        {
            foreach (var mob in Dungeon.Level.mobs.Where(mob => mob != ch))
                mob.Beckon(pos);

            if (Dungeon.Visible[pos])
            {
                GLog.Warning("The trap emits a piercing sound that echoes throughout the dungeon!");
                CellEmitter.Center(pos).Start(Speck.Factory(Speck.SCREAM), 0.3f, 3);
            }

            Sample.Instance.Play(Assets.SND_ALERT);
        }
    }
}