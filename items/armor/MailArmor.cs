using sharpdungeon.sprites;

namespace sharpdungeon.items.armor
{
    public class MailArmor : Armor
    {
        public MailArmor()
            : base(3)
        {
            name = "mail armor";
            image = ItemSpriteSheet.ARMOR_MAIL;
        }

        public override string Desc()
        {
            return "Interlocking metal links make for a tough but flexible suit of armor.";
        }
    }
}