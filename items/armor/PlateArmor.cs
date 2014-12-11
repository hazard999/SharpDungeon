using sharpdungeon.sprites;

namespace sharpdungeon.items.armor
{
	public class PlateArmor : Armor
	{
		public PlateArmor() : base(5)
		{
            name = "plate armor";
            image = ItemSpriteSheet.ARMOR_PLATE;
		}

		public override string Desc()
		{
			return "Enormous plates of metal are joined together into a suit that provides " + "unmatched protection to any adventurer strong enough to bear its staggering weight.";
		}
	}
}