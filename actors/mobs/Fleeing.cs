
namespace sharpdungeon.actors.mobs
{
    public class Fleeing : IAiState
    {
        protected readonly Mob Mob;

        public Fleeing(Mob mob)
        {
            Mob = mob;
        }

        public const string Tag = "FLEEING";

        public bool Act(bool enemyInFov, bool justAlerted)
        {
            Mob.EnemySeen = enemyInFov;
            if (enemyInFov)
                Mob.Target = Mob.Enemy.pos;

            var oldPos = Mob.pos;
            if (Mob.Target != -1 && Mob.GetFurther(Mob.Target))
            {
                Mob.Spend(1 / Mob.Speed());

                return Mob.MoveSprite(oldPos, Mob.pos);
            }

            Mob.Spend(Actor.Tick);

            NowhereToRun();

            return true;
        }

        protected virtual void NowhereToRun()
        {
        }

        public string Status()
        {
            return string.Format("This {0} is fleeing", Mob.Name);
        }
    }
}