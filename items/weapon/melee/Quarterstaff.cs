using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class Quarterstaff : MeleeWeapon
    {
        public Quarterstaff()
            : base(2, 1f, 1f)
        {
            name = "quarterstaff";
            image = ItemSpriteSheet.QUARTERSTAFF;
        }

        public override string Desc()
        {
            return "A staff of hardwood, its ends are shod with iron.";
        }
    }
}