using sharpdungeon.sprites;
namespace sharpdungeon.items.armor
{
    public class ScaleArmor : Armor
    {
        public ScaleArmor()
            : base(4)
        {
            name = "scale armor";
            image = ItemSpriteSheet.ARMOR_SCALE;
        }

        public override string Desc()
        {
            return "The metal scales sewn onto a leather vest create a flexible, yet protective armor.";
        }
    }
}