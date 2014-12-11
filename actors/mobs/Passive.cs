namespace sharpdungeon.actors.mobs
{
    public class Passive : IAiState
    {
        private readonly Mob _mob;

        public Passive(Mob mob)
        {
            _mob = mob;
        }

        public const string Tag = "PASSIVE";

        public bool Act(bool enemyInFov, bool justAlerted)
        {
            _mob.EnemySeen = false;
            
            _mob.Spend(Actor.Tick);

            return true;
        }

        public string Status()
        {
            return string.Format("This {0} is passive", _mob.Name);
        }
    }
}