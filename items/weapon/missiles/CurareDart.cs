using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.missiles
{
    public class CurareDart : MissileWeapon
    {
        public const float Duration = 3f;

        public CurareDart()
            : this(1)
        {
            name = "curare dart";
            image = ItemSpriteSheet.CURARE_DART;

            Str = 14;

            Min = 1;
            Max = 3;
        }

        public CurareDart(int number)
        {
            quantity = number;
        }

        public override void Proc(Character attacker, Character defender, int damage)
        {
            Buff.Prolong<Paralysis>(defender, Duration);
            base.Proc(attacker, defender, damage);
        }

        public override string Desc()
        {
            return "These little evil darts don't do much damage but they can paralyze " + "the target leaving it helpless and motionless for some time.";
        }

        public override Item Random()
        {
            quantity = pdsharp.utils.Random.Int(2, 5);
            return this;
        }

        public override int Price()
        {
            return 12 * Quantity();
        }
    }
}