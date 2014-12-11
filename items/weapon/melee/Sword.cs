using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class Sword : MeleeWeapon
    {
        public Sword()
            : base(3, 1f, 1f)
        {
            name = "sword";
            image = ItemSpriteSheet.SWORD;
        }

        public override string Desc()
        {
            return "The razor-sharp length of steel blade shines reassuringly.";
        }
    }
}