using pdsharp.noosa.audio;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.sprites;
using System;

namespace sharpdungeon.items
{
	public class Dewdrop : Item
	{
		private const string TxtValue = "%+dHP";

	    public Dewdrop()
	    {
            name = "dewdrop";
            image = ItemSpriteSheet.DEWDROP;

            Stackable = true;
	    }

		public override bool DoPickUp(Hero hero)
		{
			var vial = hero.Belongings.GetItem<DewVial>();

		    if (hero.HP < hero.HT || vial == null || vial.IsFull)
		    {
		        var value = 1 + (Dungeon.Depth - 1)/5;
		        if (hero.heroClass == HeroClass.Huntress)
		            value++;

		        var effect = Math.Min(hero.HT - hero.HP, value*quantity);
		        if (effect > 0)
		        {
		            hero.HP += effect;
		            hero.Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), 1);
		            hero.Sprite.ShowStatus(CharSprite.Positive, TxtValue, effect);
		        }
		    }
		    else
		        vial.collectDew(this);

		    Sample.Instance.Play(Assets.SND_DEWDROP);
			hero.SpendAndNext(TimeToPickUp);

			return true;
		}

		public override string Info()
		{
			return "A crystal clear dewdrop.";
		}
	}
}