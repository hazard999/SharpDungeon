using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Senior : Monk
    {
        public Senior()
        {
            Name = "senior monk";
            SpriteClass = typeof(SeniorSprite);
        }

        public override int DamageRoll()
        {
            return Random.NormalIntRange(12, 20);
        }

        public override int AttackProc(Character enemy, int damage)
        {
            if (Random.Int(10) == 0)
                buffs.Buff.Prolong<Paralysis>(enemy, 1.1f);

            return base.AttackProc(enemy, damage);
        }

        public override void Die(object cause)
        {
            base.Die(cause);
            Badge.ValidateRare(this);
        }
    }
}