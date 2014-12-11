using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.utils;

namespace sharpdungeon.items.potions
{
    public class PotionOfHealing : Potion
    {
        public PotionOfHealing()
        {
            name = "Potion of Healing";
        }

        protected internal override void Apply(Hero hero)
        {
            SetKnown();
            Heal(Dungeon.Hero);
            GLog.Positive("Your wounds heal completely.");
        }

        public static void Heal(Hero hero)
        {
            hero.HP = hero.HT;
            Buff.Detach<Poison>(hero);
            Buff.Detach<Cripple>(hero);
            Buff.Detach<Weakness>(hero);
            Buff.Detach<Bleeding>(hero);

            hero.Sprite.Emitter().Start(Speck.Factory(Speck.HEALING), 0.4f, 4);
        }

        public override string Desc()
        {
            return "An elixir that will instantly return you to full health and cure poison.";
        }

        public override int Price()
        {
            return IsKnown ? 30 * Quantity() : base.Price();
        }
    }
}