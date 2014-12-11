using sharpdungeon.sprites;

namespace sharpdungeon.items.armor
{
    public class LeatherArmor : Armor
    {
        public LeatherArmor()
            : base(2)
        {
            name = "leather armor";
            image = ItemSpriteSheet.ARMOR_LEATHER;
        }

        public override string Desc()
        {
            return "Armor made from tanned monster hide. Not as light as cloth armor but provides better protection.";
        }
    }
}