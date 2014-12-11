namespace sharpdungeon.actors.mobs
{
    public class Hunting : IAiState
    {
        private readonly Mob _mob;

        public Hunting(Mob _mob)
        {
            this._mob = _mob;
        }

        public const string TAG = "HUNTING";

        public bool Act(bool enemyInFov, bool justAlerted)
        {

            _mob.EnemySeen = enemyInFov;

            if (enemyInFov && _mob.CanAttack(_mob.Enemy))
                return _mob.DoAttack(_mob.Enemy);

            if (enemyInFov)
                _mob.Target = _mob.Enemy.pos;


            var oldPos = _mob.pos;

            if (_mob.Target != -1 && _mob.GetCloser(_mob.Target))
            {
                _mob.Spend(1 / _mob.Speed());

                return _mob.MoveSprite(oldPos, _mob.pos);
            }

            _mob.Spend(Actor.Tick);

            _mob.State = _mob.WANDERING;

            _mob.Target = Dungeon.Level.RandomDestination();

            return true;
        }

        public string Status()
        {
            return string.Format("This {0} is hunting", _mob.Name);
        }
    }
}