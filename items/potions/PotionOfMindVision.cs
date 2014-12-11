using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.utils;

namespace sharpdungeon.items.potions
{
    public class PotionOfMindVision : Potion
    {
        public PotionOfMindVision()
        {
            name = "Potion of Mind Vision";
        }

        protected internal override void Apply(Hero hero)
        {
            SetKnown();
            Buff.Affect<MindVision>(hero, MindVision.Duration);
            Dungeon.Observe();

            if (Dungeon.Level.mobs.Count > 0)
                GLog.Information("You can somehow feel the presence of other creatures' minds!");
            else
                GLog.Information("You can somehow tell that you are alone on this Level at the moment.");
        }

        public override string Desc()
        {
            return "After drinking this, your mind will become attuned to the psychic signature " + "of distant creatures, enabling you to sense biological presences through walls. " + "Also this potion will permit you to see through nearby walls and doors.";
        }

        public override int Price()
        {
            return IsKnown ? 35 * Quantity() : base.Price();
        }
    }
}