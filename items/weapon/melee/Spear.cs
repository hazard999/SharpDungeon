using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class Spear : MeleeWeapon
    {
        public Spear()
            : base(2, 1f, 1.5f)
        {
            name = "spear";
            image = ItemSpriteSheet.SPEAR;
        }

        public override string Desc()
        {
            return "A slender wooden rod tipped with sharpened iron.";
        }
    }
}