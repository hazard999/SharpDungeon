namespace sharpdungeon.actors.mobs
{
    public class Wandering : IAiState
    {
        private readonly Mob _mob;

        public Wandering(Mob mob)
        {
            _mob = mob;
        }

        public const string Tag = "WANDERING";

        public bool Act(bool enemyInFov, bool justAlerted)
        {
            if (enemyInFov && (justAlerted || pdsharp.utils.Random.Int(_mob.Distance(_mob.Enemy) / 2 + _mob.Enemy.Stealth()) == 0))
            {
                _mob.EnemySeen = true;

                _mob.Notice();
                _mob.State = _mob.HUNTING;
                _mob.Target = _mob.Enemy.pos;

            }
            else
            {
                _mob.EnemySeen = false;

                var oldPos = _mob.pos;

                if (_mob.Target != -1 && _mob.GetCloser(_mob.Target))
                {
                    _mob.Spend(1 / _mob.Speed());
                    return _mob.MoveSprite(oldPos, _mob.pos);
                }

                _mob.Target = Dungeon.Level.RandomDestination();

                _mob.Spend(Actor.Tick);
            }
            return true;
        }

        public string Status()
        {
            return string.Format("This {0} is wandering", _mob.Name);
        }
    }
}