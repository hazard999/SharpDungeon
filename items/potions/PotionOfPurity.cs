using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.levels;
using sharpdungeon.utils;

namespace sharpdungeon.items.potions
{
	public class PotionOfPurity : Potion
	{
		private const string TXT_FRESHNESS = "You feel uncommon freshness in the air.";
		private const string TXT_NO_SMELL = "You've stopped sensing any smells!";

		private const int DISTANCE = 2;

	    public PotionOfPurity()
	    {
            name = "Potion of Purification";
	    }

		protected internal override void Shatter(int cell)
		{
			PathFinder.BuildDistanceMap(cell, BArray.not(Level.losBlocking, null), DISTANCE);

			var procd = false;

			Blob[] blobs = { Dungeon.Level.Blobs[typeof(ToxicGas)], Dungeon.Level.Blobs[typeof(ParalyticGas)] };

			foreach (var blob in blobs)
			{
			    if (blob == null)
			        continue;

			    for (var i=0; i < Level.Length; i++)
			    {
			        if (PathFinder.Distance[i] >= int.MaxValue) 
                        continue;

			        var value = blob.Cur[i];
			        if (value <= 0) 
                        continue;

			        blob.Cur[i] = 0;
			        blob.Volume -= value;
			        procd = true;

			        CellEmitter.Get(i).Burst(Speck.Factory(Speck.DISCOVER), 1);
			    }
			}

			var heroAffected = PathFinder.Distance[Dungeon.Hero.pos] < int.MaxValue;

			if (procd)
			{
				splash(cell);
				Sample.Instance.Play(Assets.SND_SHATTER);

				SetKnown();

			    if (heroAffected)
			        GLog.Positive(TXT_FRESHNESS);
            }
			else
			{
				base.Shatter(cell);

			    if (!heroAffected) 
                    return;

			    GLog.Information(TXT_FRESHNESS);
			    SetKnown();
			}
		}

		protected internal override void Apply(Hero hero)
		{
			GLog.Warning(TXT_NO_SMELL);
			Buff.Prolong<GasesImmunity>(hero, GasesImmunity.Duration);
			SetKnown();
		}

		public override string Desc()
		{
			return "This reagent will quickly neutralize All harmful gases in the area of effect. " + "Drinking it will give you a temporary immunity to such gases.";
		}

		public override int Price()
		{
			return IsKnown ? 50 * Quantity() : base.Price();
		}
	}
}