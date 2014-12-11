using System;
using System.Collections.Generic;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items
{
    public class DewVial : Item
    {
        public DewVial()
        {
            name = "dew vial";
            image = ItemSpriteSheet.VIAL;

            DefaultAction = AcDrink;

            unique = true;
        }

        private const int MaxVolume = 10;

        private const string AcDrink = "DRINK";

        private const float TimeToDrink = 1f;

        private const string TxtValue = "{0}HP";
        private const string TxtStatus = "{0}/{1}";

        private const string TxtAutoDrink = "The dew vial was emptied to heal your wounds.";
        private const string TxtCollected = "You collected a dewdrop into your dew vial.";
        private const string TxtFull = "Your dew vial is full!";
        private const string TxtEmpty = "Your dew vial is empty!";
        
        private int _volume;

        private const string Volume = "volume";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(Volume, _volume);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            _volume = bundle.GetInt(Volume);
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            if (_volume > 0)
                actions.Add(AcDrink);
            return actions;
        }

        private const double NUM = 20;
        private readonly double _pow = Math.Log10(NUM);

        public override void Execute(Hero hero, string action)
        {
            if (action.Equals(AcDrink))
            {
                if (_volume > 0)
                {
                    var value = (int) Math.Ceiling(Math.Pow(_volume, _pow)/NUM*hero.HT);
                    var effect = Math.Min(hero.HT - hero.HP, value);
                    if (effect > 0)
                    {
                        hero.HP += effect;
                        hero.Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), _volume > 5 ? 2 : 1);
                        hero.Sprite.ShowStatus(CharSprite.Positive, TxtValue, effect);
                    }

                    _volume = 0;

                    hero.Spend(TimeToDrink);
                    hero.Busy();

                    Sample.Instance.Play(Assets.SND_DRINK);
                    hero.Sprite.DoOperate(hero.pos);

                    UpdateQuickslot();
                }
                else
                    GLog.Warning(TxtEmpty);
            }
            else
                base.Execute(hero, action);
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

        public virtual bool IsFull
        {
            get
            {
                return _volume >= MaxVolume;
            }
        }

        public virtual void collectDew(Dewdrop dew)
        {

            GLog.Information(TxtCollected);
            _volume += dew.quantity;
            if (_volume >= MaxVolume)
            {
                _volume = MaxVolume;
                GLog.Positive(TxtFull);
            }

            UpdateQuickslot();
        }

        public virtual void Fill()
        {
            _volume = MaxVolume;
            UpdateQuickslot();
        }

        public static void AutoDrink(Hero hero)
        {
            var vial = hero.Belongings.GetItem<DewVial>();
            if (vial == null || !vial.IsFull) 
                return;
            vial.Execute(hero);
            hero.Sprite.Emitter().Start(ShaftParticle.Factory, 0.2f, 3);

            GLog.Warning(TxtAutoDrink);
        }

        private static readonly ItemSprite.Glowing White = new ItemSprite.Glowing(0xFFFFCC);

        public override ItemSprite.Glowing Glowing()
        {
            return IsFull ? White : null;
        }

        public override string Status()
        {
            return Utils.Format(TxtStatus, _volume, MaxVolume);
        }

        public override string Info()
        {
            return "You can store excess dew in this tiny vessel for drinking it later. " + "If the vial is full, in a moment of deadly peril the dew will be " + "consumed automatically.";
        }

        public override string ToString()
        {
            return base.ToString() + " (" + Status() + ")";
        }
    }
}