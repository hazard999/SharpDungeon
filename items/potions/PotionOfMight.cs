using sharpdungeon.actors.hero;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items.potions
{
    public class PotionOfMight : PotionOfStrength
    {
        public PotionOfMight()
        {
            name = "Potion of Might";
        }

        protected internal override void Apply(Hero hero)
        {
            SetKnown();

            hero.STR++;
            hero.HT += 5;
            hero.HP += 5;
            hero.Sprite.ShowStatus(CharSprite.Positive, "+1 str, +5 ht");
            GLog.Positive("Newfound strength surges through your body.");
        }

        public override string Desc()
        {
            return "This powerful liquid will course through your muscles, permanently " + "increasing your strength by one point and health by five points.";
        }

        public override int Price()
        {
            return IsKnown ? 200 * quantity : base.Price();
        }
    }
}