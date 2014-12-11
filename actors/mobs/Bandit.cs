using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.items;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs
{
    public class Bandit : Thief
    {
        public new Item Item;

        public Bandit()
        {
            Name = "crazy bandit";
            SpriteClass = typeof(BanditSprite);
        }

        protected internal override bool Steal(Hero hero)
        {
            if (!base.Steal(hero))
                return false;

            buffs.Buff.Prolong<Blindness>(Enemy, pdsharp.utils.Random.Int(5, 12));
            Dungeon.Observe();

            return true;
        }

        public override void Die(object cause)
        {
            base.Die(cause);
            Badge.ValidateRare(this);
        }
    }
}