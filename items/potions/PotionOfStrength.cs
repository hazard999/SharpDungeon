using sharpdungeon.actors.hero;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items.potions
{
    public class PotionOfStrength : Potion
    {
        public PotionOfStrength()
        {
            name = "Potion of Strength";
        }

        protected internal override void Apply(Hero hero)
        {
            SetKnown();

            hero.STR++;
            hero.Sprite.ShowStatus(CharSprite.Positive, "+1 str");
            GLog.Positive("Newfound strength surges through your body.");

            Badge.ValidateStrengthAttained();
        }

        public override string Desc()
        {
            return "This powerful liquid will course through your muscles, " + "permanently increasing your strength by one point.";
        }

        public override int Price()
        {
            return IsKnown ? 100 * Quantity() : base.Price();
        }
    }
}