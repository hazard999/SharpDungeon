using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.utils;

namespace sharpdungeon.items.potions
{
    public class PotionOfLevitation : Potion
    {
        public PotionOfLevitation()
        {
            name = "Potion of Levitation";
        }

        protected internal override void Apply(Hero hero)
        {
            SetKnown();
            Buff.Affect<Levitation>(hero, Levitation.Duration);
            GLog.Information("You float into the air!");
        }

        public override string Desc()
        {
            return "Drinking this curious liquid will cause you to hover in the air, " + "able to drift effortlessly over traps. Flames and gases " + "fill the air, however, and cannot be bypassed while airborne.";
        }

        public override int Price()
        {
            return IsKnown ? 35 * Quantity() : base.Price();
        }
    }

}