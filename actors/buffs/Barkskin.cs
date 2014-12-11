using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
    public class Barkskin : Buff
    {
        private int _level;

        protected override bool Act()
        {
            if (Target.IsAlive)
            {
                Spend(Tick);
                if (--_level <= 0)
                    Detach();
            }
            else
                Detach();

            return true;
        }

        public virtual int Level()
        {
            return _level;
        }

        public virtual void Level(int value)
        {
            if (_level < value)
                _level = value;
        }

        public override int Icon()
        {
            return BuffIndicator.BARKSKIN;
        }

        public override string ToString()
        {
            return "Barkskin";
        }
    }
}