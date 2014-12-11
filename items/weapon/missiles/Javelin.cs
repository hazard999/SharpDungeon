using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.missiles
{
    public class Javelin : MissileWeapon
    {
        public Javelin()
            : this(1)
        {
        }

        public Javelin(int number)
        {
            quantity = number;
            name = "javelin";
            image = ItemSpriteSheet.JAVELIN;

            Str = 15;

            Min = 2;
            Max = 15;
        }

        public override void Proc(Character attacker, Character defender, int damage)
        {
            base.Proc(attacker, defender, damage);
            Buff.Prolong<Cripple>(defender, Cripple.Duration);
        }

        public override string Desc()
        {
            return "This length of metal is weighted to keep the spike " + "at its tip foremost as it sails through the air.";
        }

        public override Item Random()
        {
            quantity = pdsharp.utils.Random.Int(5, 15);
            return this;
        }

        public override int Price()
        {
            return 15 * Quantity();
        }
    }
}