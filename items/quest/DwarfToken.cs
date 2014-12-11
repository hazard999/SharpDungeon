using sharpdungeon.sprites;

namespace sharpdungeon.items.quest
{
	public class DwarfToken : Item
	{
	    public DwarfToken()
	    {
            name = "dwarf token";
            image = ItemSpriteSheet.TOKEN;

            Stackable = true;
            unique = true;
	    }

		public override bool Upgradable
		{
			get
			{
				return false;
			}
		}

		public override bool Identified
		{
			get
			{
				return true;
			}
		}

		public override string Info()
		{
			return "Many dwarves and some of their larger creations carry these small pieces of metal of unknown purpose. " + "Maybe they are jewelry or maybe some kind of ID. Dwarves are strange folk.";
		}

		public override int Price()
		{
			return quantity * 100;
		}
	}

}