using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class Knuckles : MeleeWeapon
    {
        public Knuckles()
            : base(1, 1f, 0.5f)
        {
            name = "knuckleduster";
            image = ItemSpriteSheet.KNUCKLEDUSTER;
        }

        public override string Desc()
        {
            return "A piece of iron shaped to fit around the knuckles.";
        }
    }
}