using System;
using System.Globalization;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.sprites;
using sharpdungeon.ui;

namespace sharpdungeon.items.armor.glyphs
{
    public class Metabolism : Glyph
	{
		private const string TxtMetabolism = "{0} of metabolism";

		private static readonly ItemSprite.Glowing Red = new ItemSprite.Glowing(0xCC0000);

		public override int Proc(Armor armor, Character attacker, Character defender, int damage)
		{
			var level = Math.Max(0, armor.level);
		    
            if (pdsharp.utils.Random.Int(level/2 + 5) < 4) 
                return damage;

		    var healing = Math.Min(defender.HT - defender.HP, pdsharp.utils.Random.Int(1, defender.HT / 5));

		    if (healing <= 0) 
                return damage;

		    var hunger = defender.Buff<Hunger>();

		    if (hunger == null || hunger.IsStarving) 
                return damage;

		    hunger.Satisfy(-Hunger.Starving / 10);
		    BuffIndicator.RefreshHero();

		    defender.HP += healing;
		    defender.Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), 1);
		    defender.Sprite.ShowStatus(CharSprite.Positive, healing.ToString(CultureInfo.InvariantCulture));

		    return damage;
		}

		public override string Name(string weaponName)
		{
			return string.Format(TxtMetabolism, weaponName);
		}

		public override ItemSprite.Glowing Glowing()
		{
			return Red;
		}
	}
}