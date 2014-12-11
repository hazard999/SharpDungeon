using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class WarHammer : MeleeWeapon
    {
        public WarHammer()
            : base(5, 1.2f, 1f)
        {
            name = "war hammer";
            image = ItemSpriteSheet.WAR_HAMMER;
        }

        public override string Desc()
        {
            return "Few creatures can withstand the crushing blow of this towering mass of lead and steel, " + "but only the strongest of adventurers can use it effectively.";
        }
    }
}