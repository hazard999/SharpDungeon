using System;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.levels;
using sharpdungeon.sprites;

namespace sharpdungeon.items.armor.glyphs
{
    public class AntiEntropy : Glyph
	{
		private const string TxtAntiEntropy = "{0} of anti-entropy";

		private static readonly ItemSprite.Glowing Blue = new ItemSprite.Glowing(0x0000FF);

		public override int Proc(Armor armor, Character attacker, Character defender, int damage)
		{
			var level = Math.Max(0, armor.level);

		    if (!Level.Adjacent(attacker.pos, defender.pos) || pdsharp.utils.Random.Int(level + 6) < 5) 
                return damage;

		    Buff.Prolong<Frost>(attacker, Frost.duration(attacker) * pdsharp.utils.Random.Float(1f, 1.5f));
		    CellEmitter.Get(attacker.pos).Start(SnowParticle.Factory, 0.2f, 6);

		    Buff.Affect<Burning>(defender).Reignite(defender);
		    defender.Sprite.Emitter().Burst(FlameParticle.Factory, 5);

		    return damage;
		}

		public override string Name(string weaponName)
		{
			return string.Format(TxtAntiEntropy, weaponName);
		}

		public override ItemSprite.Glowing Glowing()
		{
			return Blue;
		}
	}
}