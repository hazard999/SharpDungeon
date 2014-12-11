using sharpdungeon.effects;
using sharpdungeon.items;
using sharpdungeon.items.potions;
using sharpdungeon.items.rings;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.wands;
using sharpdungeon.items.weapon;
using sharpdungeon.items.weapon.melee;
using sharpdungeon.plants;

namespace sharpdungeon.actors.blobs
{
	public class WaterOfTransmutation : WellWater
	{
		protected internal override Item AffectItem(Item item)
		{
		    if (item is MeleeWeapon)
		        return ChangeWeapon((MeleeWeapon) item);

		    if (item is Scroll)
		    {
		        Journal.Remove(Journal.Feature.WELL_OF_TRANSMUTATION);
		        return ChangeScroll((Scroll)item);
		    }

		    if (item is Potion)
		    {
		        Journal.Remove(Journal.Feature.WELL_OF_TRANSMUTATION);
		        return ChangePotion((Potion)item);
		    }
		    
            if (item is Ring)
		    {
		        Journal.Remove(Journal.Feature.WELL_OF_TRANSMUTATION);
		        return ChangeRing((Ring)item);
		    }
		    
            if (item is Wand)
		    {
		        Journal.Remove(Journal.Feature.WELL_OF_TRANSMUTATION);
		        return ChangeWand((Wand)item);
		    }
            
		    if (item is Plant.Seed)
		    {
		        Journal.Remove(Journal.Feature.WELL_OF_TRANSMUTATION);
		        return ChangeSeed((Plant.Seed)item);
		    }

		    return null;
		}

	    public override void Use(BlobEmitter emitter)
		{
			base.Use(emitter);
			emitter.Start(Speck.Factory(Speck.CHANGE), 0.2f, 0);
		}

		private MeleeWeapon ChangeWeapon(MeleeWeapon w)
		{
			MeleeWeapon n = null;

		    if (w is Knuckles)
		        n = new Dagger();
		    else if (w is Dagger)
		        n = new Knuckles();
		    else if (w is Spear)
		        n = new Quarterstaff();
		    else if (w is Quarterstaff)
		        n = new Spear();
		    else if (w is Sword)
		        n = new Mace();
		    else if (w is Mace)
		        n = new Sword();
		    else if (w is Longsword)
		        n = new BattleAxe();
		    else if (w is BattleAxe)
		        n = new Longsword();
		    else if (w is Glaive)
		        n = new WarHammer();
		    else if (w is WarHammer)
		        n = new Glaive();

		    if (n == null) 
                return null;

		    var level = w.level;
		    if (level > 0)
		        n.Upgrade(level);
		    else if (level < 0)
		        n.Degrade(-level);

		    if (w.IsEnchanted)
		        n.Enchant(Weapon.Enchantment.Random());

		    n.levelKnown = w.levelKnown;
		    n.cursedKnown = w.cursedKnown;
		    n.cursed = w.cursed;

		    Journal.Remove(Journal.Feature.WELL_OF_TRANSMUTATION);

		    return n;
		}

		private Ring ChangeRing(Ring r)
		{
			Ring n;
			do
			{
				n = (Ring)Generator.Random(Generator.Category.RING);
			} while (n.GetType() == r.GetType());

			n.level = 0;

			var level = r.level;
		    if (level > 0)
		        n.Upgrade(level);
		    else if (level < 0)
		        n.Degrade(-level);

		    n.levelKnown = r.levelKnown;
			n.cursedKnown = r.cursedKnown;
			n.cursed = r.cursed;

			return n;
		}

		private static Wand ChangeWand(Wand w)
		{
			Wand n;
			do
			{
				n = (Wand)Generator.Random(Generator.Category.WAND);
			} while (n.GetType() == w.GetType());

			n.Level = 0;
			n.Upgrade(w.Level);

			n.levelKnown = w.levelKnown;
			n.cursedKnown = w.cursedKnown;
			n.cursed = w.cursed;

			return n;
		}

		private static Plant.Seed ChangeSeed(Plant.Seed s)
		{

			Plant.Seed n;

			do
			{
				n = (Plant.Seed)Generator.Random(Generator.Category.SEED);
			} while (n.GetType() == s.GetType());

			return n;
		}

		private Scroll ChangeScroll(Scroll s)
		{
		    if (s is ScrollOfUpgrade)
		        return new ScrollOfWeaponUpgrade();

		    if (s is ScrollOfWeaponUpgrade)
		        return new ScrollOfUpgrade();
		    Scroll n;
		    do
		    {
		        n = (Scroll) Generator.Random(Generator.Category.SCROLL);
		    } while (n.GetType() == s.GetType());
		    return n;
		}

	    private Potion ChangePotion(Potion p)
	    {
	        if (p is PotionOfStrength)
	            return new PotionOfMight();
	        
            if (p is PotionOfMight)
	            return new PotionOfStrength();
	        
            Potion n;
	        do
	        {
	            n = (Potion) Generator.Random(Generator.Category.POTION);
	        } while (n.GetType() == p.GetType());
	        return n;
	    }

	    public override string TileDesc()
		{
			return "Power of change radiates from the water of this well. " + "Throw an item into the well to turn it into something else.";
		}
	}
}