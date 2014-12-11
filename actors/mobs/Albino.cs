using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Albino : Rat
    {
        public Albino()
        {
            Name = "albino rat";
            SpriteClass = typeof(AlbinoSprite);

            HP = HT = 15;
        }

        public override void Die(object cause)
        {
            base.Die(cause);
            Badge.ValidateRare(this);
        }

        public override int AttackProc(Character enemy, int damage)
        {
            if (Random.Int(2) == 0)
                buffs.Buff.Affect<Bleeding>(enemy).Set(damage);

            return damage;
        }
    }
}