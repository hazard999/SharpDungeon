using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.items.potions;
using sharpdungeon.sprites;
using sharpdungeon.ui;

namespace sharpdungeon.plants
{
    public class Earthroot : Plant
    {
        private const string TxtDesc = "When a creature touches an Earthroot, its roots " + "create a kind of natural armor around it.";

        public Earthroot()
        {
            Image = 5;
            PlantName = "Earthroot";
        }

        public override void Activate(Character ch)
        {
            base.Activate(ch);

            if (ch != null)
                Buff.Affect<Armor>(ch).Level = ch.HT;

            if (!Dungeon.Visible[Pos])
                return;

            CellEmitter.Bottom(Pos).Start(effects.particles.EarthParticle.Factory, 0.05f, 8);
            Camera.Main.Shake(1, 0.4f);
        }

        public override string Desc()
        {
            return TxtDesc;
        }

        public new class Seed : Plant.Seed
        {
            public Seed()
            {
                plantName = "Earthroot";

                name = "seed of " + plantName;
                image = ItemSpriteSheet.SEED_EARTHROOT;

                PlantClass = typeof(Earthroot);
                AlchemyClass = typeof(PotionOfParalyticGas);
            }

            public override string Desc()
            {
                return TxtDesc;
            }
        }

        public class Armor : Buff
        {
            private const float Step = 1f;

            private int _pos;
            private int _level;

            public override bool AttachTo(Character target)
            {
                _pos = target.pos;
                return base.AttachTo(target);
            }

            protected override bool Act()
            {
                if (Target.pos != _pos)
                    Detach();

                Spend(Step);
                return true;
            }

            public virtual int Absorb(int damage)
            {
                if (damage >= _level)
                {
                    Detach();
                    return damage - _level;
                }

                _level -= damage;
                return 0;
            }

            public virtual int Level
            {
                get { return _level; }
                set
                {
                    if (_level < value)
                        _level = value;
                }
            }

            public override int Icon()
            {
                return BuffIndicator.ARMOR;
            }

            public override string ToString()
            {
                return "Herbal armor";
            }

            private const string POS = "pos";
            private const string LEVEL = "Level";

            public override void StoreInBundle(Bundle bundle)
            {
                base.StoreInBundle(bundle);
                bundle.Put(POS, _pos);
                bundle.Put(LEVEL, _level);
            }

            public override void RestoreFromBundle(Bundle bundle)
            {
                base.RestoreFromBundle(bundle);
                _pos = bundle.GetInt(POS);
                _level = bundle.GetInt(LEVEL);
            }
        }
    }
}