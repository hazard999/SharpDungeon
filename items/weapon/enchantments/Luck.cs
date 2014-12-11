using Java.Lang;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Luck : Weapon.Enchantment
    {
        private const string TxtLucky = "Lucky {0}";

        private static readonly ItemSprite.Glowing Green = new ItemSprite.Glowing(0x00FF00);

        public override bool Proc(Weapon weapon, actors.Character attacker, actors.Character defender, int damage)
        {
            var level = Math.Max(0, weapon.level);

            var dmg = damage;
            for (var i = 1; i <= level + 1; i++)
                dmg = Math.Max(dmg, attacker.DamageRoll() - i);

            if (dmg <= damage)
                return false;

            defender.Damage(dmg - damage, this);
            return true;
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtLucky, weaponName);
        }

        public override ItemSprite.Glowing Glowing()
        {
            return Green;
        }
    }
}