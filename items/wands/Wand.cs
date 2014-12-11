using System.Collections.Generic;
using System.Text;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.items.bags;
using sharpdungeon.items.rings;
using sharpdungeon.mechanics;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.actors;
using sharpdungeon.effects;
using System;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.items.wands
{
    public abstract class Wand : KindOfWeapon
    {
        public const string AcZap = "ZAP";

        private const string TxtWood = "This thin {0} wand is warm to the touch. Who knows what it will do when used?";
        private const string TxtDamage = "When this wand is used as a melee weapon, its average damage is {0} points per hit.";
        private const string TxtWeapon = "You can use this wand as a melee weapon.";

        public const string TxtFizzles = "your wand fizzles; it must be out of charges for now";
        public const string TxtSelfTarget = "You can't target yourself";

        public const float TimeToZap = 1f;

        public int MaxCharges;
        public int CurrrentCharges;

        protected internal Charger ChargerInstance;

        private bool _curChargeKnown;

        protected internal bool HitChars = true;

        private static readonly Type[] Wands =
        {
            typeof(WandOfTeleportation), 
            typeof(WandOfSlowness), 
            typeof(WandOfFirebolt), 
            typeof(WandOfPoison), 
            typeof(WandOfRegrowth), 
            typeof(WandOfBlink), 
            typeof(WandOfLightning), 
            typeof(WandOfAmok), 
            typeof(WandOfTelekinesis), 
            typeof(WandOfFlock), 
            typeof(WandOfDisintegration), 
            typeof(WandOfAvalanche)
        };

        private static readonly string[] Woods = { "holly", "yew", "ebony", "cherry", "teak", "rowan", "willow", "mahogany", "bamboo", "purpleheart", "oak", "birch" };
        private static readonly int?[] Images = { ItemSpriteSheet.WAND_HOLLY, ItemSpriteSheet.WAND_YEW, ItemSpriteSheet.WAND_EBONY, ItemSpriteSheet.WAND_CHERRY, ItemSpriteSheet.WAND_TEAK, ItemSpriteSheet.WAND_ROWAN, ItemSpriteSheet.WAND_WILLOW, ItemSpriteSheet.WAND_MAHOGANY, ItemSpriteSheet.WAND_BAMBOO, ItemSpriteSheet.WAND_PURPLEHEART, ItemSpriteSheet.WAND_OAK, ItemSpriteSheet.WAND_BIRCH };

        private static ItemStatusHandler _handler;

        private readonly string _wood;
        public static void InitWoods()
        {
            _handler = new ItemStatusHandler(Wands, Woods, Images);
        }

        public static void Save(Bundle bundle)
        {
            _handler.Save(bundle);
        }

        public static void Restore(Bundle bundle)
        {
            _handler = new ItemStatusHandler(Wands, Woods, Images, bundle);
        }

        protected Wand()
        {
            MaxCharges = InitialCharges();
            CurrrentCharges = MaxCharges;
            DefaultAction = AcZap;
            CalculateDamage();
            Zapper = new ZapperCellSelectorListener(this);
            try
            {
                if (_handler == null)
                    return;

                image = _handler.Image(GetType()) ?? 0;
                _wood = _handler.Label(GetType());
            }
            catch (Exception)
            {
                // Wand of Magic Factory
            }
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);

            if (CurrrentCharges > 0 || !_curChargeKnown)
                actions.Add(AcZap);

            if (hero.heroClass == HeroClass.Mage)
                return actions;

            actions.Remove(AcEquip);
            actions.Remove(AcUnequip);
            return actions;
        }

        public override bool DoUnequip(Hero hero, bool collect)
        {
            OnDetach();
            return base.DoUnequip(hero, collect);
        }

        public override void Activate(Hero hero)
        {
            Charge(hero);
        }

        public override void Execute(Hero hero, string action)
        {
            if (action.Equals(AcZap))
            {
                CurUser = hero;
                curItem = this;
                GameScene.SelectCell(Zapper);
            }
            else
                base.Execute(hero, action);
        }

        protected internal abstract void OnZap(int cell);

        public override bool Collect(Bag container)
        {
            if (!base.Collect(container))
                return false;

            if (container.Owner != null)
                Charge(container.Owner);

            return true;
        }

        public virtual void Charge(Character owner)
        {
            (ChargerInstance = new Charger(this)).AttachTo(owner);
        }

        protected override void OnDetach()
        {
            StopCharging();
        }

        public virtual void StopCharging()
        {
            if (ChargerInstance == null)
                return;

            ChargerInstance.Detach();
            ChargerInstance = null;
        }

        public virtual int Level
        {
            get
            {
                if (ChargerInstance == null)
                    return Level;

                var power = ChargerInstance.Target.Buff<RingOfPower.Power>();
                return power == null ? Level : System.Math.Max(Level + power.Level, 0);
            }
            set { Level = value; }
        }

        protected internal virtual bool IsKnown
        {
            get
            {
                if (_handler != null)
                    return false;

                return _handler.IsKnown(GetType());
            }
        }

        public virtual void SetKnown()
        {
            if (!IsKnown)
                if (_handler != null)
                    _handler.Know(GetType());

            Badge.ValidateAllWandsIdentified();
        }

        public override Item Identify()
        {
            SetKnown();
            _curChargeKnown = true;
            base.Identify();

            UpdateQuickslot();

            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(base.ToString());

            var status = Status();
            if (status != null)
                sb.Append(" (" + status + ")");

            return sb.ToString();
        }

        public override string Name
        {
            get { return IsKnown ? name : _wood + " wand"; }
        }

        public override string Info()
        {
            var info = new StringBuilder(IsKnown ? Desc() : string.Format(TxtWood));

            if (Dungeon.Hero.heroClass != HeroClass.Mage)
                return info.ToString();

            info.Append("\\Negative\\Negative");

            if (levelKnown)
                info.Append(string.Format(TxtDamage));
            else
                info.Append(string.Format(TxtWeapon));

            return info.ToString();
        }

        public override bool Identified
        {
            get
            {
                return base.Identified && IsKnown && _curChargeKnown;
            }
        }

        public override string Status()
        {
            if (!levelKnown)
                return null;

            return _curChargeKnown ? CurrrentCharges.ToString() : "?" + "/" + MaxCharges;
        }

        public override Item Upgrade()
        {
            base.Upgrade();

            UpdateLevel();
            CurrrentCharges = System.Math.Min(CurrrentCharges + 1, MaxCharges);
            UpdateQuickslot();

            return this;
        }

        public override Item Degrade()
        {
            base.Degrade();

            UpdateLevel();
            UpdateQuickslot();

            return this;
        }

        protected internal virtual void UpdateLevel()
        {
            MaxCharges = System.Math.Min(InitialCharges() + Level, 9);
            CurrrentCharges = System.Math.Min(CurrrentCharges, MaxCharges);

            CalculateDamage();
        }

        protected internal virtual int InitialCharges()
        {
            return 2;
        }

        private void CalculateDamage()
        {
            var tier = 1 + Level / 3;
            Min = tier;
            Max = (tier * tier - tier + 10) / 2 + Level;
        }

        protected internal virtual void Fx(int cell, ICallback callback)
        {
            MagicMissile.BlueLight(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        protected internal virtual void WandUsed()
        {
            CurrrentCharges--;

            UpdateQuickslot();

            CurUser.SpendAndNext(TimeToZap);
        }

        public override Item Random()
        {
            if (!(pdsharp.utils.Random.Float() < 0.5f))
                return this;

            Upgrade();
            if (pdsharp.utils.Random.Float() < 0.15f)
                Upgrade();

            return this;
        }

        public static bool AllKnown()
        {
            if (_handler != null)
                return _handler.Known().Count == Wands.Length;

            return false;
        }

        public override int Price()
        {
            var price = 50;
            if (cursed && cursedKnown)
                price /= 2;

            if (levelKnown)
                if (Level > 0)
                    price *= (Level + 1);
                else if (Level < 0)
                    price /= (1 - Level);

            if (price < 1)
                price = 1;

            return price;
        }

        private const string MAX_CHARGES = "maxCharges";
        private const string CUR_CHARGES = "curCharges";
        private const string CUR_CHARGE_KNOWN = "curChargeKnown";

        public override void StoreInBundle(Bundle bundle)
        {
            base.StoreInBundle(bundle);
            bundle.Put(MAX_CHARGES, MaxCharges);
            bundle.Put(CUR_CHARGES, CurrrentCharges);
            bundle.Put(CUR_CHARGE_KNOWN, _curChargeKnown);
        }

        public override void RestoreFromBundle(Bundle bundle)
        {
            base.RestoreFromBundle(bundle);
            MaxCharges = bundle.GetInt(MAX_CHARGES);
            CurrrentCharges = bundle.GetInt(CUR_CHARGES);
            _curChargeKnown = bundle.GetBoolean(CUR_CHARGE_KNOWN);
        }


        protected static CellSelector.Listener Zapper;

        protected internal class Charger : Buff
        {
            private readonly Wand _wand;

            public Charger(Wand wand)
            {
                _wand = wand;
            }

            private const float TIME_TO_CHARGE = 40f;

            public override bool AttachTo(Character target)
            {
                base.AttachTo(target);
                Delay();

                return true;
            }

            protected override bool Act()
            {
                if (_wand.CurrrentCharges < _wand.MaxCharges)
                {
                    _wand.CurrrentCharges++;
                    _wand.UpdateQuickslot();
                }

                Delay();

                return true;
            }

            protected internal virtual void Delay()
            {
                var time2Charge = ((Hero)Target).heroClass == HeroClass.Mage ? TIME_TO_CHARGE / (float)Math.Sqrt(1 + _wand.Level) : TIME_TO_CHARGE;
                Spend(time2Charge);
            }
        }
    }

    public class ZapperCellSelectorListener : CellSelector.Listener, ICallback
    {
        private readonly Wand _wand;
        private int _cell;

        public ZapperCellSelectorListener(Wand wand)
        {
            _wand = wand;
        }

        public void OnSelect(int? target)
        {
            if (target == null)
                return;

            if (target == _wand.CurUser.pos)
            {
                GLog.Information(Wand.TxtSelfTarget);
                return;
            }

            _wand.SetKnown();

            _cell = Ballistica.Cast(_wand.CurUser.pos, target.Value, true, _wand.HitChars);
            _wand.CurUser.Sprite.DoZap(_cell);

            QuickSlot.Target(_wand, Actor.FindChar(_cell));

            if (_wand.CurrrentCharges > 0)
            {
                _wand.CurUser.Busy();

                _wand.Fx(_cell, this);

                Invisibility.Dispel();
            }
            else
            {
                _wand.CurUser.SpendAndNext(Wand.TimeToZap);
                GLog.Warning(Wand.TxtFizzles);
                _wand.levelKnown = true;

                _wand.UpdateQuickslot();
            }
        }

        public string Prompt()
        {
            return "Choose direction to DoZap";
        }

        public void Call()
        {
            _wand.OnZap(_cell);
            _wand.WandUsed();
        }
    }
}