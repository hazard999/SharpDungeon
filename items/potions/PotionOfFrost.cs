using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.levels;
using sharpdungeon.utils;

namespace sharpdungeon.items.potions
{
	public class PotionOfFrost : Potion
	{
		private const int Distance = 2;

	    public PotionOfFrost()
	    {
            name = "Potion of Frost";
	    }

        protected internal override void Shatter(int cell)
		{
			PathFinder.BuildDistanceMap(cell, BArray.not(Level.losBlocking, null), Distance);

			var fire = (Fire)Dungeon.Level.Blobs[typeof(Fire)];

            for (var i = 0; i < Level.Length; i++)
                if (PathFinder.Distance[i] < int.MaxValue)
                    Freezing.Affect(i, fire);

            splash(cell);
			Sample.Instance.Play(Assets.SND_SHATTER);

			SetKnown();
		}

		public override string Desc()
		{
			return "Upon exposure to open air this chemical will evaporate into a freezing cloud, causing " + "any creature that contacts it to be frozen in place unable to act and move.";
		}

		public override int Price()
		{
			return IsKnown ? 50 * Quantity() : base.Price();
		}
	}
}