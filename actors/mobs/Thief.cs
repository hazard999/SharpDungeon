using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.items;
using sharpdungeon.items.rings;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.actors.mobs
{
    public class Thief : Mob
    {
        protected internal const string TxtStole = "{0} stole {1} from you!";
        protected internal const string TxtCarries = "\\Negative\\Negative{0} is carrying a _{1}_. Stolen obviously.";

        public Item Item;

        public Thief()
        {
            Name = "crazy thief";
            SpriteClass = typeof(ThiefSprite);

            HP = HT = 20;
            defenseSkill = 12;

            Exp = 5;
            MaxLvl = 10;

            loot = new RingOfHaggler();
            lootChance = 0.01f;

            FLEEING = new ThiefFleeing(this);
        }

        private const string ITEM = "item";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(ITEM, Item);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            Item = (Item)bundle.Get(ITEM);
        }

        public override int DamageRoll()
        {
            return Random.NormalIntRange(1, 7);
        }

        protected internal override float AttackDelay()
        {
            return 0.5f;
        }

        public override void Die(object cause)
        {
            base.Die(cause);

            if (Item != null)
                Dungeon.Level.Drop(Item, pos).Sprite.Drop();
        }

        public override int AttackSkill(Character target)
        {
            return 12;
        }

        public override int Dr()
        {
            return 3;
        }

        public override int AttackProc(Character enemy, int damage)
        {
            if (Item == null && enemy is Hero && Steal((Hero)enemy))
                State = FLEEING;

            return damage;
        }

        public override int DefenseProc(Character enemy, int damage)
        {
            if (State == FLEEING)
                Dungeon.Level.Drop(new Gold(), pos).Sprite.Drop();

            return damage;
        }

        protected internal virtual bool Steal(Hero hero)
        {
            var item = hero.Belongings.RandomUnequipped();
            if (item == null)
                return false;

            GLog.Warning(TxtStole, Name, item.Name);

            item.DetachAll(hero.Belongings.Backpack);
            Item = item;

            return true;
        }

        public override string Description()
        {
            var desc = "Deeper levels of the dungeon have always been a hiding place for All kinds of criminals. " + "Not All of them could keep a clear mind during their extended periods so far from daylight. Long ago, " + "these crazy thieves and bandits have forgotten who they are and why they steal.";

            if (Item != null)
                desc += string.Format(TxtCarries, Utils.Capitalize(Name), Item.Name);

            return desc;
        }

        private class ThiefFleeing : Fleeing
        {
            public ThiefFleeing(Mob mob)
                : base(mob)
            {
            }

            protected override void NowhereToRun()
            {
                if (Mob.Buff<Terror>() == null)
                {
                    Mob.Sprite.ShowStatus(CharSprite.Negative, TxtRage);
                    Mob.State = Mob.HUNTING;
                }
                else
                    base.NowhereToRun();
            }
        }
    }
}