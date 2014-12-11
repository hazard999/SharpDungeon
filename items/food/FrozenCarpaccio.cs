using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using System;

namespace sharpdungeon.items.food
{
	public class FrozenCarpaccio : Food
	{
	    public FrozenCarpaccio()
	    {
            name = "frozen carpaccio";
            image = ItemSpriteSheet.CARPACCIO;
            Energy = Hunger.Starving - Hunger.Hungry;
	    }

		public override void Execute(Hero hero, string action)
		{
			base.Execute(hero, action);

		    if (!action.Equals(AcEat)) 
                return;

		    switch (pdsharp.utils.Random.Int(5))
		    {
		        case 0:
		            GLog.Information("You see your hands turn invisible!");
                    Buff.Affect<Invisibility>(hero, Invisibility.Duration);
		            break;
		        case 1:
		            GLog.Information("You feel your skin hardens!");
		            Buff.Affect<Barkskin>(hero).Level(hero.HT / 4);
		            break;
		        case 2:
		            GLog.Information("Refreshing!");
		            Buff.Detach<Poison>(hero);
		            Buff.Detach<Cripple>(hero);
		            Buff.Detach<Weakness>(hero);
		            Buff.Detach<Bleeding>(hero);
		            break;
		        case 3:
		            GLog.Information("You feel better!");
		            if (hero.HP < hero.HT)
		            {
		                hero.HP = Math.Min(hero.HP + hero.HT / 4, hero.HT);
		                hero.Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), 1);
		            }
		            break;
		    }
		}

		public override string Info()
		{
			return "It's a piece of frozen raw meat. The only way to eat it is " + "by cutting thin slices of it. And this way it's suprisingly good.";
		}

		public override int Price()
		{
			return 10 * Quantity();
		}

		public static Food Cook(MysteryMeat ingredient)
		{
			var result = new FrozenCarpaccio();
			result.quantity = ingredient.Quantity();
			return result;
		}
	}
}