using System;
using pdsharp.utils;
using sharpdungeon.ui;
using sharpdungeon.utils;
using sharpdungeon.effects;

namespace sharpdungeon.actors.buffs
{
    public class Bleeding : Buff
	{
		protected internal int Level;

		private const string LEVEL = "Level";

		public override void StoreInBundle(Bundle bundle)
		{
			base.StoreInBundle(bundle);
			bundle.Put(LEVEL, Level);

		}

		public override void RestoreFromBundle(Bundle bundle)
		{
			base.RestoreFromBundle(bundle);
			Level = bundle.GetInt(LEVEL);
		}

		public virtual void Set(int level)
		{
			Level = level;
		}

		public override int Icon()
		{
			return BuffIndicator.BLEEDING;
		}

		public override string ToString()
		{
			return "Bleeding";
		}

        protected override bool Act()
		{
            if (Target.IsAlive)
            {
                if ((Level = pdsharp.utils.Random.Int(Level/2, Level)) > 0)
                {
                    Target.Damage(Level, this);
                    if (Target.Sprite.IsVisible)
                        Splash.At(Target.Sprite.Center(), -PointF.Pi/2, PointF.Pi/6, Target.Sprite.Blood(),
                            Math.Min(10*Level/Target.HT, 10));

                    if (Target == Dungeon.Hero && !Target.IsAlive)
                    {
                        Dungeon.Fail(Utils.Format(ResultDescriptions.BLEEDING, Dungeon.Depth));
                        GLog.Negative("You bled to death...");
                    }

                    Spend(Tick);
                }
                else
                    Detach();
            }
            else
                Detach();

            return true;
		}
	}
}