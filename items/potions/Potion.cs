using System;
using System.Collections.Generic;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.items.potions
{
    public class Potion : Item
    {
        public const string AC_DRINK = "DRINK";

        private const string TXT_HARMFUL = "Harmful potion!";
        private const string TXT_BENEFICIAL = "Beneficial potion";
        private const string TXT_YES = "Yes, I know what I'm doing";
        private const string TXT_NO = "No, I changed my mind";
        private const string TXT_R_U_SURE_DRINK = "Are you sure you want to drink it? In most cases you should throw such potions at your enemies.";
        private const string TXT_R_U_SURE_THROW = "Are you sure you want to throw it? In most cases it makes sense to drink it.";

        private const float TIME_TO_DRINK = 1f;

        private static readonly Type[] potions = { typeof(PotionOfHealing), typeof(PotionOfExperience), typeof(PotionOfToxicGas), typeof(PotionOfLiquidFlame), typeof(PotionOfStrength), typeof(PotionOfParalyticGas), typeof(PotionOfLevitation), typeof(PotionOfMindVision), typeof(PotionOfPurity), typeof(PotionOfInvisibility), typeof(PotionOfMight), typeof(PotionOfFrost) };
        private static readonly string[] colors = { "turquoise", "crimson", "azure", "jade", "golden", "magenta", "charcoal", "ivory", "amber", "bistre", "indigo", "silver" };
        private static readonly int?[] images = { ItemSpriteSheet.POTION_TURQUOISE, ItemSpriteSheet.POTION_CRIMSON, ItemSpriteSheet.POTION_AZURE, ItemSpriteSheet.POTION_JADE, ItemSpriteSheet.POTION_GOLDEN, ItemSpriteSheet.POTION_MAGENTA, ItemSpriteSheet.POTION_CHARCOAL, ItemSpriteSheet.POTION_IVORY, ItemSpriteSheet.POTION_AMBER, ItemSpriteSheet.POTION_BISTRE, ItemSpriteSheet.POTION_INDIGO, ItemSpriteSheet.POTION_SILVER };

        private static ItemStatusHandler _handler;

        private readonly string _color;

        public static void InitColors()
        {
            _handler = new ItemStatusHandler(potions, colors, images);
        }

        public static void Save(Bundle bundle)
        {
            _handler.Save(bundle);
        }

        public static void Restore(Bundle bundle)
        {
            _handler = new ItemStatusHandler(potions, colors, images, bundle);
        }

        public Potion()
        {
            Stackable = true;
            DefaultAction = AC_DRINK;

            if (_handler == null)
                return;

            image = _handler.Image(GetType()) ?? 0;
            _color = _handler.Label(GetType());
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(AC_DRINK);
            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action.Equals(AC_DRINK))
            {
                if (IsKnown && (this is PotionOfLiquidFlame || this is PotionOfToxicGas || this is PotionOfParalyticGas))
                {
                    //GameScene.show(new WndOptions(TXT_HARMFUL, TXT_R_U_SURE_DRINK, TXT_YES, TXT_NO) { protected void onSelect(int index) { if (index == 0) { drink(hero); } }; });
                }
                else
                    Drink(hero);
            }
            else
                base.Execute(hero, action);
        }

        public override void DoThrow(Hero hero)
        {
            if (IsKnown && (this is PotionOfExperience || this is PotionOfHealing || this is PotionOfLevitation || this is PotionOfMindVision || this is PotionOfStrength || this is PotionOfInvisibility || this is PotionOfMight))
            {
                //GameScene.show(new WndOptions(TXT_BENEFICIAL, TXT_R_U_SURE_THROW, TXT_YES, TXT_NO) { protected void onSelect(int index) { if (index == 0) { Potion.super.doThrow(hero); } }; });
            }
            else
                base.DoThrow(hero);
        }

        protected internal virtual void Drink(Hero hero)
        {
            Detach(hero.Belongings.Backpack);

            hero.Spend(TIME_TO_DRINK);
            hero.Busy();
            OnThrow(hero.pos);

            Sample.Instance.Play(Assets.SND_DRINK);

            hero.Sprite.DoOperate(hero.pos);
        }

        protected override void OnThrow(int cell)
        {
            if (Dungeon.Hero.pos == cell)
                Apply(Dungeon.Hero);
            else if (Dungeon.Level.map[cell] == Terrain.WELL || Level.pit[cell])
                base.OnThrow(cell);
            else
                Shatter(cell);
        }

        protected internal virtual void Apply(Hero hero)
        {
            Shatter(hero.pos);
        }

        protected internal virtual void Shatter(int cell)
        {
            GLog.Information("The flask shatters and " + _color + " liquid splashes harmlessly");
            Sample.Instance.Play(Assets.SND_SHATTER);
            splash(cell);
        }

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

            Badge.ValidateAllPotionsIdentified();
        }

        public override Item Identify()
        {
            SetKnown();
            return this;
        }

        protected internal virtual string Color()
        {
            return _color;
        }

        public override string Name
        {
            get { return IsKnown ? name : _color + " potion"; }
        }

        public override string Info()
        {
            return IsKnown ? Desc() : "This flask contains a swirling " + _color + " liquid. " + "Who knows what it will do when drunk or thrown?";
        }

        public bool IsIdentified
        {
            get
            {
                return IsKnown;
            }
        }

        public override bool Upgradable
        {
            get
            {
                return false;
            }
        }

        public static HashSet<Type> GetKnown
        {
            get
            {
                return _handler.Known();
            }
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
            return _handler.Known().Count == potions.Length;
        }

        protected internal virtual void splash(int cell)
        {
            var color = ItemSprite.Pick(image, 8, 10);
            Splash.At(cell, color, 5);
        }

        public override int Price()
        {
            return 20 * quantity;
        }
    }
}