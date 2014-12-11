using sharpdungeon.sprites;

namespace sharpdungeon.items.armor
{
    public class ClothArmor : Armor
    {
        public ClothArmor()
            : base(1)
        {
            name = "cloth armor";
            image = ItemSpriteSheet.ARMOR_CLOTH;
        }

        public override string Desc()
        {
            return "This lightweight armor offers basic protection.";
        }
    }
}