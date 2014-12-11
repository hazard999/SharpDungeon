using pdsharp.noosa.audio;
using sharpdungeon.actors;
using sharpdungeon.actors.blobs;
using sharpdungeon.scenes;

namespace sharpdungeon.items.potions
{
	public class PotionOfToxicGas : Potion
	{
	    public PotionOfToxicGas()
	    {
            name = "Potion of Toxic Gas";
	    }

		protected internal override void Shatter(int cell)
		{
			SetKnown();

			splash(cell);
			Sample.Instance.Play(Assets.SND_SHATTER);

			var gas = Blob.Seed(cell, 1000, typeof(ToxicGas));
			Actor.Add(gas);
			GameScene.Add(gas);
		}

		public override string Desc()
		{
			return "Uncorking or shattering this pressurized glass will cause " + "its contents to explode into a deadly cloud of toxic green gas. " + "You might choose to fling this potion at distant enemies " + "instead of uncorking it by hand.";
		}

		public override int Price()
		{
			return IsKnown ? 40 * Quantity() : base.Price();
		}
	}
}