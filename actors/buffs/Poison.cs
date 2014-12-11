using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.items.rings;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.actors.buffs
{
    public class Poison : Buff, Hero.IDoom
	{
		protected internal float left;

		private const string LEFT = "left";

		public override void StoreInBundle(Bundle bundle)
		{
			base.StoreInBundle(bundle);
			bundle.Put(LEFT, left);
		}

		public override void RestoreFromBundle(Bundle bundle)
		{
			base.RestoreFromBundle(bundle);
			left = bundle.GetFloat(LEFT);
		}

		public virtual void Set(float duration)
		{
			left = duration;
		}

		public override int Icon()
		{
			return BuffIndicator.POISON;
		}

		public override string ToString()
		{
			return "Poisoned";
		}

        protected override bool Act()
		{
		    if (Target.IsAlive)
		    {
		        Target.Damage((int) (left/3) + 1, this);
		        Spend(Tick);

		        if ((left -= Tick) <= 0)
		        {
		            Detach();
		        }
		    }
		    else
		        Detach();

		    return true;
		}

		public static float DurationFactor(Character ch)
		{
			var r = ch.Buff<RingOfElements.Resistance>();
			return r != null ? r.DurationFactor() : 1;
		}

		public void OnDeath()
		{
			Badge.ValidateDeathFromPoison();

			Dungeon.Fail(Utils.Format(ResultDescriptions.POISON, Dungeon.Depth));
			GLog.Negative("You died from poison...");
		}
	}
}