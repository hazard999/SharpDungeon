using sharpdungeon.actors;
using sharpdungeon.items.weapon.missiles;

namespace sharpdungeon.items.weapon.enchantments
{
    public class Instability : Weapon.Enchantment
    {
        private const string TxtUnstable = "Unstable {0}";

        public override bool Proc(Weapon weapon, Character attacker, Character defender, int damage)
        {
            var ench = Random();
            
            if (!(weapon is Boomerang)) 
                return ench.Proc(weapon, attacker, defender, damage);

            while (ench is Piercing || ench is Swing)
                ench = Random();

            return ench.Proc(weapon, attacker, defender, damage);
        }

        public override string Name(string weaponName)
        {
            return string.Format(TxtUnstable, weaponName);
        }
    }
}