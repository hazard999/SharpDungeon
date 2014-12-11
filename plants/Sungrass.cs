using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items.potions;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using System;

namespace sharpdungeon.plants
{
    public class Sungrass : Plant
    {
        private const string TxtDesc = "Sungrass is renowned for its sap's healing properties.";

        public Sungrass()
        {
            Image = 4;
            PlantName = "Sungrass";
        }

        public override void Activate(Character ch)
        {
            base.Activate(ch);

            if (ch != null)
                Buff.Affect<Health>(ch);

            if (Dungeon.Visible[Pos])
                CellEmitter.Get(Pos).Start(ShaftParticle.Factory, 0.2f, 3);
        }

        public override string Desc()
        {
            return TxtDesc;
        }

        public new class Seed : Plant.Seed
        {
            public Seed()
            {
                plantName = "Sungrass";

                name = "seed of " + plantName;
                image = ItemSpriteSheet.SEED_SUNGRASS;

                PlantClass = typeof(Sungrass);
                AlchemyClass = typeof(PotionOfHealing);
            }

            public override string Desc()
            {
                return TxtDesc;
            }
        }

        public class Health : Buff
        {
            private const float STEP = 5f;

            private int pos;

            public override bool AttachTo(Character target)
            {
                pos = target.pos;
                return base.AttachTo(target);
            }

            protected override bool Act()
            {
                if (Target.pos != pos || Target.HP >= Target.HT)
                    Detach();
                else
                {
                    Target.HP = Math.Min(Target.HT, Target.HP + Target.HT / 10);
                    Target.Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), 1);
                }

                Spend(STEP);
                return true;
            }

            public override int Icon()
            {
                return BuffIndicator.HEALING;
            }

            public override string ToString()
            {
                return "Herbal healing";
            }

            private const string POS = "pos";

            public override void StoreInBundle(Bundle bundle)
            {
                base.StoreInBundle(bundle);
                bundle.Put(POS, pos);
            }

            public override void RestoreFromBundle(Bundle bundle)
            {
                base.RestoreFromBundle(bundle);
                pos = bundle.GetInt(POS);
            }
        }
    }
}