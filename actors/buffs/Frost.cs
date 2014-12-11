using sharpdungeon.actors.hero;
using sharpdungeon.items.food;
using sharpdungeon.items.rings;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class Frost : FlavourBuff
	{
		private const float Duration = 5f;

		public override bool AttachTo(Character target)
		{
		    if (!base.AttachTo(target)) return false;
		    target.Paralysed = true;
		    
		    Buff.Detach<Burning>(target);

		    var hero = target as Hero;
		    if (hero == null)
		        return true;
			   
		    var item = hero.Belongings.RandomUnequipped();
		    if (!(item is MysteryMeat)) 
		        return true;

		    item.Detach(hero.Belongings.Backpack);
			    
		    var carpaccio = new FrozenCarpaccio();
		    if (!carpaccio.Collect(hero.Belongings.Backpack))
		        Dungeon.Level.Drop(carpaccio, hero.pos).Sprite.Drop();

		    return true;
		}

	    public override void Detach()
		{
			Target.Paralysed = false;
			base.Detach();
		}

		public override int Icon()
		{
			return BuffIndicator.FROST;
		}

		public override string ToString()
		{
			return "Frozen";
		}

		public static float duration(Character ch)
		{
			var r = ch.Buff<RingOfElements.Resistance>();
			return r != null ? r.DurationFactor() * Duration : Duration;
		}
	}
}