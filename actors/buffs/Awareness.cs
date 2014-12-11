namespace sharpdungeon.actors.buffs
{
    public class Awareness : FlavourBuff
	{
		public const float Duration = 2f;

		public override void Detach()
		{
			base.Detach();
			Dungeon.Observe();
		}
	}
}