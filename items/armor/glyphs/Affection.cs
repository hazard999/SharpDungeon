using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.levels;
using sharpdungeon.sprites;

namespace sharpdungeon.items.armor.glyphs
{
	public class Affection : Glyph
	{
		private const string TXT_AFFECTION = "{0} of affection";

		private static readonly ItemSprite.Glowing Pink = new ItemSprite.Glowing(0xFF4488);

		public override int Proc(Armor armor, Character attacker, Character defender, int damage)
		{
			var level = (int)GameMath.Gate(0, armor.level, 6);

		    if (!Level.Adjacent(attacker.pos, defender.pos) || pdsharp.utils.Random.Int(level/2 + 5) < 4) 
                return damage;

		    var duration = pdsharp.utils.Random.IntRange(2, 5);

		    Buff.Affect<Charm>(attacker, Charm.durationFactor(attacker) * duration);
		    attacker.Sprite.CenterEmitter().Start(Speck.Factory(Speck.HEART), 0.2f, 5);

		    Buff.Affect<Charm>(defender, pdsharp.utils.Random.Float(Charm.durationFactor(defender) * duration / 2, duration));
		    defender.Sprite.CenterEmitter().Start(Speck.Factory(Speck.HEART), 0.2f, 5);

		    return damage;
		}

		public override string Name(string weaponName)
		{
			return string.Format(TXT_AFFECTION, weaponName);
		}

		public override ItemSprite.Glowing Glowing()
		{
			return Pink;
		}
	}
}