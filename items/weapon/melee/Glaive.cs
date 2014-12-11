using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class Glaive : MeleeWeapon
    {
        public Glaive()
            : base(5, 1f, 1f)
        {
            name = "glaive";
            image = ItemSpriteSheet.GLAIVE;
        }

        public override string Desc()
        {
            return "A polearm consisting of a sword blade on the end of a pole.";
        }
    }
}