using pdsharp.utils;
using sharpdungeon.actors.blobs;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.items.food;
using sharpdungeon.items.rings;
using sharpdungeon.items.scrolls;
using sharpdungeon.scenes;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.actors.buffs
{
    public class Burning : Buff, Hero.IDoom
    {
        private const string TXT_BURNS_UP = "{0} burns up!";
        private const string TXT_BURNED_TO_DEATH = "You burned to death...";

        private const float DURATION = 8f;

        private float left;

        private const string LEFT = "left";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(LEFT, left);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            left = bundle.GetFloat(LEFT);
        }

        protected override bool Act()
        {
            if (Target.IsAlive)
            {
                if (Target is Hero)
                    Prolong<Light>(Target, Tick * 1.01f);

                Target.Damage(pdsharp.utils.Random.Int(1, 5), this);

                var hero = Target as Hero;
                if (hero != null)
                {
                    var item = hero.Belongings.RandomUnequipped();
                    if (item is Scroll)
                    {
                        item = item.Detach(hero.Belongings.Backpack);
                        GLog.Warning(TXT_BURNS_UP, item.ToString());

                        Heap.BurnFx(hero.pos);
                    }
                    else if (item is MysteryMeat)
                    {
                        item = item.Detach(hero.Belongings.Backpack);
                        var steak = new ChargrilledMeat();
                        if (!steak.Collect(hero.Belongings.Backpack))
                            Dungeon.Level.Drop(steak, hero.pos).Sprite.Drop();
                        GLog.Warning(TXT_BURNS_UP, item.ToString());

                        Heap.BurnFx(hero.pos);
                    }
                }
                else if (Target is Thief && ((Thief)Target).Item is Scroll)
                {
                    ((Thief)Target).Item = null;
                    Target.Sprite.Emitter().Burst(ElmoParticle.Factory, 6);
                }
            }
            else
                Detach();

            if (levels.Level.flamable[Target.pos])
                GameScene.Add(Blob.Seed(Target.pos, 4, typeof(Fire)));

            Spend(Tick);
            left -= Tick;

            if (left <= 0 || Random.Float() > (2 + (float)Target.HP / Target.HT) / 3 || (levels.Level.water[Target.pos] && !Target.Flying))
                Detach();

            return true;
        }

        public virtual void Reignite(Character ch)
        {
            left = Duration(ch);
        }

        public override int Icon()
        {
            return BuffIndicator.FIRE;
        }

        public override string ToString()
        {
            return "Burning";
        }

        public void OnDeath()
        {
            Badge.ValidateDeathFromFire();

            Dungeon.Fail(Utils.Format(ResultDescriptions.BURNING, Dungeon.Depth));
            GLog.Negative(TXT_BURNED_TO_DEATH);
        }

        public static float Duration(Character ch)
        {
            var r = ch.Buff<RingOfElements.Resistance>();
            return r != null ? r.DurationFactor() * DURATION : DURATION;
        }
    }
}