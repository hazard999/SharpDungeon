using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class MindVision : FlavourBuff
	{
		public const float Duration = 20f;

		public int Distance = 2;

		public override int Icon()
		{
			return BuffIndicator.MIND_VISION;
		}

		public override string ToString()
		{
			return "Mind vision";
		}

		public override void Detach()
		{
			base.Detach();
			Dungeon.Observe();
		}
	}
}