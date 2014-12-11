using System;
using System.Collections.Generic;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.levels;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using Random = pdsharp.utils.Random;

namespace sharpdungeon.plants
{
    public class Plant : Bundlable
    {
        public string PlantName;

        public int Image;
        public int Pos;

        public PlantSprite Sprite;

        public virtual void Activate(Character ch)
        {
            if (ch is Hero && ((Hero)ch).subClass == HeroSubClass.WARDEN)
                Buff.Affect<Barkskin>(ch).Level(ch.HT / 3);

            Wither();
        }

        public virtual void Wither()
        {
            Dungeon.Level.Uproot(Pos);

            Sprite.Kill();
            if (Dungeon.Visible[Pos])
                CellEmitter.Get(Pos).Burst(LeafParticle.Factory, 6);

            if (Dungeon.Hero.subClass != HeroSubClass.WARDEN)
                return;

            if (Random.Int(5) == 0)
                Dungeon.Level.Drop(Generator.Random(Generator.Category.SEED), Pos).Sprite.Drop();
            if (Random.Int(5) == 0)
                Dungeon.Level.Drop(new Dewdrop(), Pos).Sprite.Drop();
        }

        private const string POS = "pos";

        public void RestoreFromBundle(Bundle bundle)
        {
            Pos = bundle.GetInt(POS);
        }

        public void StoreInBundle(Bundle bundle)
        {
            bundle.Put(POS, Pos);
        }

        public virtual string Desc()
        {
            return null;
        }

        public class Seed : Item
        {
            public const string AcPlant = "PLANT";

            private const string TxtInfo = "Throw this seed to the place where you want to grow {0}.\\Negative\\Negative{1}";

            private const float TimeToPlant = 1f;

            public Seed()
            {
                Stackable = true;
                DefaultAction = AcThrow;
            }

            protected internal Type PlantClass;
            public string plantName;


            public Type AlchemyClass;

            public override List<string> Actions(Hero hero)
            {
                var actions = base.Actions(hero);
                actions.Add(AcPlant);
                return actions;
            }

            protected override void OnThrow(int cell)
            {
                if (Dungeon.Level.map[cell] == Terrain.ALCHEMY || Level.pit[cell])
                    base.OnThrow(cell);
                else
                    Dungeon.Level.Plant(this, cell);
            }

            public override void Execute(Hero hero, string action)
            {
                if (action.Equals(AcPlant))
                {
                    hero.Spend(TimeToPlant);
                    hero.Busy();
                    ((Seed)Detach(hero.Belongings.Backpack)).OnThrow(hero.pos);

                    hero.Sprite.DoOperate(hero.pos);
                }
                else
                    base.Execute(hero, action);
            }

            public virtual Plant Couch(int pos)
            {
                try
                {
                    Sample.Instance.Play(Assets.SND_PLANT);
                    var plant = (Plant)Activator.CreateInstance(PlantClass);

                    plant.Pos = pos;
                    return plant;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public override bool Upgradable
            {
                get
                {
                    return false;
                }
            }

            public override bool Identified
            {
                get
                {
                    return true;
                }
            }

            public override int Price()
            {
                return 10 * quantity;
            }

            public override string Info()
            {
                return string.Format(TxtInfo, Utils.Indefinite(plantName), Desc());
            }
        }
    }

}