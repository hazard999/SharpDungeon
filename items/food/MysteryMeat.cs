using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items.food
{
    public class MysteryMeat : Food
    {
        public MysteryMeat()
        {
            name = "mystery meat";
            image = ItemSpriteSheet.MEAT;
            Energy = Hunger.Starving - Hunger.Hungry;
            Message = "That food tasted... strange.";
        }

        public override void Execute(Hero hero, string action)
        {
            base.Execute(hero, action);

            if (!action.Equals(AcEat))
                return;

            switch (pdsharp.utils.Random.Int(5))
            {
                case 0:
                    GLog.Warning("Oh it's hot!");
                    Buff.Affect<Burning>(hero).Reignite(hero);
                    break;
                case 1:
                    GLog.Warning("You can't feel your legs!");
                    Buff.Prolong<Roots>(hero, Paralysis.Duration(hero));
                    break;
                case 2:
                    GLog.Warning("You are not feeling well.");
                    Buff.Affect<Poison>(hero).Set(Poison.DurationFactor(hero) * hero.HT / 5);
                    break;
                case 3:
                    GLog.Warning("You are stuffed.");
                    Buff.Prolong<Slow>(hero, Slow.Duration(hero));
                    break;
            }
        }

        public override string Info()
        {
            return "Eat at your own risk!";
        }

        public override int Price()
        {
            return 5 * Quantity();
        }
    }
}