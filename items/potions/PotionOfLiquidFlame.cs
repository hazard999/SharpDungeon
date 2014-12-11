using pdsharp.noosa.audio;
using sharpdungeon.actors.blobs;
using sharpdungeon.scenes;

namespace sharpdungeon.items.potions
{
	public class PotionOfLiquidFlame : Potion
	{
	    public PotionOfLiquidFlame()
	    {
            name = "Potion of Liquid Flame";
	    }

		protected internal override void Shatter(int cell)
		{
			SetKnown();

			splash(cell);
			Sample.Instance.Play(Assets.SND_SHATTER);

			var fire = Blob.Seed(cell, 2, typeof(Fire));
			GameScene.Add(fire);
		}

		public override string Desc()
		{
			return "This flask contains an unstable compound which will burst " + "violently into flame upon exposure to open air.";
		}

		public override int Price()
		{
			return IsKnown ? 40 * Quantity() : base.Price();
		}
	}
}