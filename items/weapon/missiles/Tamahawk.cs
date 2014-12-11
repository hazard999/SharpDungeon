using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.missiles
{
    public class Tamahawk : MissileWeapon
    {
        public Tamahawk()
            : this(1)
        {
        }

        public Tamahawk(int number)
        {
            quantity = number;
            name = "tomahawk";
            image = ItemSpriteSheet.TOMAHAWK;

            Str = 17;

            Min = 4;
            Max = 20;
        }

        public override void Proc(Character attacker, Character defender, int damage)
        {
            base.Proc(attacker, defender, damage);
            Buff.Affect<Bleeding>(defender).Set(damage);
        }

        public override string Desc()
        {
            return "This throwing axe is not that heavy, but it still " + "requires significant strength to be used effectively.";
        }

        public override Item Random()
        {
            quantity = pdsharp.utils.Random.Int(5, 12);
            return this;
        }

        public override int Price()
        {
            return 20 * Quantity();
        }
    }
}