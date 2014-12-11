using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class Dagger : MeleeWeapon
    {
        public Dagger()
            : base(1, 1.2f, 1f)
        {
            name = "dagger";
            image = ItemSpriteSheet.DAGGER;
        }

        public override string Desc()
        {
            return "A simple iron dagger with a well worn wooden handle.";
        }
    }
}