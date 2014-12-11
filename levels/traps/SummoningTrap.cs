using System.Collections.Generic;
using sharpdungeon.actors;
using sharpdungeon.actors.mobs;
using sharpdungeon.items.wands;
using sharpdungeon.scenes;
using pdsharp.utils;

namespace sharpdungeon.levels.traps
{
    public class SummoningTrap
    {
        private const float Delay = 2f;

        private static readonly Mob Dummy; // = new Mob();

        // 0x770088
        public static void Trigger(int pos, Character c)
        {
            if (Dungeon.BossLevel())
                return;

            if (c != null)
                Actor.OccupyCell(c);

            var nMobs = 1;
            if (Random.Int(2) == 0)
            {
                nMobs++;
                if (Random.Int(2) == 0)
                    nMobs++;
            }

            // It's complicated here, because these traps can be activated in chain
            var candidates = new List<int>();

            foreach (var neighbour in Level.NEIGHBOURS8)
            {
                var p = pos + neighbour;
                if (Actor.FindChar(p) == null && (Level.passable[p] || Level.avoid[p]))
                    candidates.Add(p);
            }

            var respawnPoints = new List<int>();

            while (nMobs > 0 && candidates.Count > 0)
            {
                var index = Random.Index(candidates);

                Dummy.pos = candidates[index];
                Actor.OccupyCell(Dummy);
                
                var cand = candidates[index];
                candidates.RemoveAt(index);
                respawnPoints.Add(cand);

                nMobs--;
            }

            foreach (var point in respawnPoints)
            {
                var mob = Bestiary.Mob(Dungeon.Depth);
                mob.State = mob.WANDERING;
                GameScene.Add(mob, Delay);
                WandOfBlink.Appear(mob, point);
            }
        }
    }
}