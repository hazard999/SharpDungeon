using pdsharp.noosa.audio;
using sharpdungeon.actors.blobs;
using sharpdungeon.scenes;

namespace sharpdungeon.items.potions
{
	public class PotionOfParalyticGas : Potion
	{
	    public PotionOfParalyticGas()
	    {
            name = "Potion of Paralytic Gas";
	    }

		protected internal override void Shatter(int cell)
		{
			SetKnown();

			splash(cell);
			Sample.Instance.Play(Assets.SND_SHATTER);

			GameScene.Add(Blob.Seed(cell, 1000, typeof(ParalyticGas)));
		}

		public override string Desc()
		{
			return "Upon exposure to open air, the liquid in this flask will vaporize " + "into a numbing yellow haze. Anyone who inhales the cloud will be paralyzed " + "instantly, unable to move for some time after the cloud dissipates. This " + "item can be thrown at distant enemies to catch them within the effect of the gas.";
		}

		public override int Price()
		{
			return IsKnown ? 40 * Quantity() : base.Price();
		}
	}
}