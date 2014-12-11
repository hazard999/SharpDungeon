using System;
using System.Globalization;
using sharpdungeon.actors;
using sharpdungeon.effects;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Leech : Weapon.Enchantment
    {
        private const string TxtVampiric = "Vampiric {0}";

        private static readonly ItemSprite.Glowing Red = new ItemSprite.Glowing(0x660022);

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            var level = Math.Max(0, weapon.level);

            // lvl 0 - 33%
            // lvl 1 - 43%
            // lvl 2 - 50%
            var maxValue = damage * (level + 2) / (level + 6);
            var effValue = Math.Min(pdsharp.utils.Random.IntRange(0, maxValue), attacker.HT - attacker.HP);

            if (effValue <= 0) 
                return false;

            attacker.HP += effValue;
            attacker.Sprite.Emitter().Start(Speck.Factory(Speck.HEALING), 0.4f, 1);
            attacker.Sprite.ShowStatus(CharSprite.Positive, effValue.ToString(CultureInfo.InvariantCulture));

            return true;
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Red;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtVampiric, weaponName);
        }
    }
}