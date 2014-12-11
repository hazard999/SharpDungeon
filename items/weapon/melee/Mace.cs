using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class Mace : MeleeWeapon
    {
        public Mace()
            : base(3, 1f, 0.8f)
        {
            name = "mace";
            image = ItemSpriteSheet.MACE;
        }

        public override string Desc()
        {
            return "The iron head of this weapon inflicts substantial damage.";
        }
    }
}