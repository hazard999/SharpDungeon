using System;
using System.Collections.Generic;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.sprites;
using pdsharp.utils;
using sharpdungeon.utils;

namespace sharpdungeon.items.scrolls
{
    public abstract class Scroll : Item
    {
        private const string TxtBlinded = "You can't read a scroll while blinded";

        public const string AcRead = "READ";

        protected internal const float TimeToRead = 1f;

        private static readonly Type[] Scrolls = { 
                                                       typeof(ScrollOfIdentify), 
                                                       typeof(ScrollOfMagicMapping), 
                                                       typeof(ScrollOfRecharging),
                                                       typeof(ScrollOfRemoveCurse),
                                                       typeof(ScrollOfTeleportation), 
                                                       typeof(ScrollOfUpgrade), 
                                                       typeof(ScrollOfChallenge),
                                                       typeof(ScrollOfTerror), 
                                                       typeof(ScrollOfLullaby),
                                                       typeof(ScrollOfWeaponUpgrade), 
                                                       typeof(ScrollOfPsionicBlast), 
                                                       typeof(ScrollOfMirrorImage) 
                                                   };
        private static readonly string[] Runes = { "KAUNAN", "SOWILO", "LAGUZ", "YNGVI", "GYFU", "RAIDO", "ISAZ", "MANNAZ", "NAUDIZ", "BERKANAN", "ODAL", "TIWAZ" };
        private static readonly int?[] Images = { ItemSpriteSheet.SCROLL_KAUNAN, ItemSpriteSheet.SCROLL_SOWILO, ItemSpriteSheet.SCROLL_LAGUZ, ItemSpriteSheet.SCROLL_YNGVI, ItemSpriteSheet.SCROLL_GYFU, ItemSpriteSheet.SCROLL_RAIDO, ItemSpriteSheet.SCROLL_ISAZ, ItemSpriteSheet.SCROLL_MANNAZ, ItemSpriteSheet.SCROLL_NAUDIZ, ItemSpriteSheet.SCROLL_BERKANAN, ItemSpriteSheet.SCROLL_ODAL, ItemSpriteSheet.SCROLL_TIWAZ };

        private static ItemStatusHandler _handler;

        private readonly string _rune;

        public static void InitLabels()
        {
            _handler = new ItemStatusHandler(Scrolls, Runes, Images);
        }

        public static void Save(Bundle bundle)
        {
            _handler.Save(bundle);
        }


        public static void Restore(Bundle bundle)
        {
            _handler = new ItemStatusHandler(Scrolls, Runes, Images, bundle);
        }

        protected Scroll()
        {
            Stackable = true;
            DefaultAction = AcRead;

            if (_handler == null)
                return;

            image = _handler.Image(GetType()) ?? 0;
            _rune = _handler.Label(GetType());
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(AcRead);
            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action.Equals(AcRead))
            {
                if (hero.Buff<Blindness>() != null)
                    GLog.Warning(TxtBlinded);
                else
                {
                    CurUser = hero;
                    curItem = Detach(hero.Belongings.Backpack);
                    DoRead();
                }
            }
            else
                base.Execute(hero, action);
        }

        protected internal abstract void DoRead();

        public virtual bool IsKnown
        {
            get
            {
                return _handler.IsKnown(GetType());
            }
        }

        public virtual void SetKnown()
        {
            if (!IsKnown)
                _handler.Know(GetType());

            Badge.ValidateAllScrollsIdentified();
        }

        public override Item Identify()
        {
            SetKnown();
            return base.Identify();
        }

        public override string Name
        {
            get { return IsKnown ? name : "scroll \"" + _rune + "\""; }
        }

        public override string Info()
        {
            return IsKnown ? Desc() : "This parchment is covered with indecipherable writing, and bears a title " + "of rune " + _rune + ". Who knows what it will do when read aloud?";
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
                return IsKnown;
            }
        }


        public static HashSet<Type> GetKnown
        {
            get { return _handler.Known(); }
        }

        public static HashSet<Type> GetUnknown
        {
            get
            {
                return _handler.Unknown();
            }
        }

        public static bool AllKnown()
        {
            return _handler.Known().Count == Scrolls.Length;
        }

        public override int Price()
        {
            return 15 * quantity;
        }
    }
}