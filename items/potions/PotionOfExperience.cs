using sharpdungeon.actors.hero;

namespace sharpdungeon.items.potions
{
    public class PotionOfExperience : Potion
    {
        public PotionOfExperience()
        {
            name = "Potion of Experience";
        }

        protected internal override void Apply(Hero hero)
        {
            SetKnown();
            hero.EarnExp(hero.MaxExp() - hero.Exp);
        }

        public override string Desc()
        {
            return "The storied experiences of multitudes of battles reduced to liquid form, " + "this draught will instantly raise your experience Level.";
        }

        public override int Price()
        {
            return IsKnown ? 80 * Quantity() : base.Price();
        }
    }
}