using System.Linq;

namespace sharpdungeon.actors.mobs
{
    public class AIStateSleeping : IAiState
    {
        private readonly Mob _mob;

        public AIStateSleeping(Mob mob)
        {
            _mob = mob;
        }

        public const string Tag = "SLEEPING";

        public bool Act(bool enemyInFov, bool justAlerted)
        {
            if (enemyInFov && pdsharp.utils.Random.Int(_mob.Distance(_mob.Enemy) + _mob.Enemy.Stealth() + (_mob.Enemy.Flying ? 2 : 0)) == 0)
            {
                _mob.EnemySeen = true;

                _mob.Notice();

                _mob.State = _mob.HUNTING;

                _mob.Target = _mob.Enemy.pos;

                if (Dungeon.IsChallenged(Challenges.SWARM_INTELLIGENCE))
                    foreach (var mob in Dungeon.Level.mobs.Where(mob => mob != _mob))
                        mob.Beckon(_mob.Target);

                _mob.Spend(Mob.TimeToWakeUp);
            }
            else
            {
                _mob.EnemySeen = false;

                _mob.Spend(Actor.Tick);
            }

            return true;
        }

        public string Status()
        {
            return string.Format("This {0} is sleeping", _mob.Name);
        }
    }
}