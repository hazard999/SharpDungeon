using System.Linq;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.items.rings;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.actors.buffs
{
    public class Hunger : Buff, Hero.IDoom
    {
        private const float Step = 10f;

        public const float Hungry = 260f;
        public const float Starving = 360f;

        private const string TxtHungry = "You are hungry.";
        private const string TxtStarving = "You are starving!";
        private const string TxtDeath = "You starved to death...";

        private float _level;

        private const string Level = "Level";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(Level, _level);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            _level = bundle.GetFloat(Level);
        }

        protected override bool Act()
        {
            if (Target.IsAlive)
            {
                var hero = (Hero) Target;

                if (IsStarving)
                {
                    if (Random.Float() < 0.3f && (Target.HP > 1 || !Target.Paralysed))
                    {
                        GLog.Negative(TxtStarving);
                        hero.Damage(1, this);

                        hero.Interrupt();
                    }
                }
                else
                {
                    var bonus = Target.Buffs<RingOfSatiety.Satiety>().Sum(buff => ((RingOfSatiety.Satiety) buff).Level);

                    var newLevel = _level + Step - bonus;
                    var statusUpdated = false;
                    if (newLevel >= Starving)
                    {
                        GLog.Negative(TxtStarving);
                        statusUpdated = true;

                        hero.Interrupt();
                    }
                    else if (newLevel >= Hungry && _level < Hungry)
                    {
                        GLog.Warning(TxtHungry);
                        statusUpdated = true;
                    }
                    _level = newLevel;

                    if (statusUpdated)
                    {
                        BuffIndicator.RefreshHero();
                    }
                }

                var step = ((Hero) Target).heroClass == HeroClass.Rogue ? Step*1.2f : Step;
                Spend(Target.Buff<Shadows>() == null ? step : step*1.5f);
            }
            else

                Deactivate();

            return true;
        }

        public virtual void Satisfy(float energy)
        {
            _level -= energy;
            if (_level < 0)
                _level = 0;
            else if (_level > Starving)
                _level = Starving;

            BuffIndicator.RefreshHero();
        }

        public virtual bool IsStarving
		{
			get
			{
				return _level >= Starving;
			}
		}

        public override int Icon()
        {
            if (_level < Hungry)
                return BuffIndicator.NONE;

            if (_level < Starving)
                return BuffIndicator.HUNGER;
            
            return BuffIndicator.STARVATION;
        }

        public override string ToString()
        {
            if (_level < Starving)
                return "Hungry";

            return "Starving";
        }

        public void OnDeath()
        {
            Badge.ValidateDeathFromHunger();

            Dungeon.Fail(Utils.Format(ResultDescriptions.HUNGER, Dungeon.Depth));
            GLog.Negative(TxtDeath);
        }
    }
}