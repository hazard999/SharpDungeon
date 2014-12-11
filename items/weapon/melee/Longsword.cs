using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class Longsword : MeleeWeapon
    {
        public Longsword()
            : base(4, 1f, 1f)
        {
            name = "longsword";
            image = ItemSpriteSheet.LONG_SWORD;
        }

        public override string Desc()
        {
            return "This towering blade inflicts heavy damage by investing its heft into every cut.";
        }
    }
}