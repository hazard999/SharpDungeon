using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class Blindness : FlavourBuff
	{
		public override void Detach()
		{
			base.Detach();
			Dungeon.Observe();
		}

		public override int Icon()
		{
			return BuffIndicator.BLINDNESS;
		}

		public override string ToString()
		{
			return "Blinded";
		}
	}
}