using System;
using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.items
{
    public class TomeOfMastery : Item
    {
        private const string TxtBlinded = "You can't read while blinded";

        public const float TimeToRead = 10;

        public const string AcRead = "READ";

        public TomeOfMastery()
        {
            Stackable = false;
            name = "Tome of Mastery";
            image = ItemSpriteSheet.MASTERY;
            unique = true;
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
                {
                    GLog.Warning(TxtBlinded);
                    return;
                }

                CurUser = hero;

                HeroSubClass way1 = null;
                HeroSubClass way2 = null;
                switch (hero.heroClass.Ordinal())
                {
                    case HeroClassType.Warrior:
                        way1 = HeroSubClass.GLADIATOR;
                        way2 = HeroSubClass.BERSERKER;
                        break;
                    case HeroClassType.Mage:
                        way1 = HeroSubClass.BATTLEMAGE;
                        way2 = HeroSubClass.WARLOCK;
                        break;
                    case HeroClassType.Rogue:
                        way1 = HeroSubClass.FREERUNNER;
                        way2 = HeroSubClass.ASSASSIN;
                        break;
                    case HeroClassType.Huntress:
                        way1 = HeroSubClass.SNIPER;
                        way2 = HeroSubClass.WARDEN;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                GameScene.Show(new WndChooseWay(this, way1, way2));
            }
            else
                base.Execute(hero, action);
        }

        public override bool DoPickUp(Hero hero)
        {
            Badge.ValidateMastery();
            return base.DoPickUp(hero);
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override bool Identified
        {
            get { return true; }
        }

        public override string Info()
        {
            return "This worn leather book is not that thick, but you feel somehow, " + "that you can gather a lot from it. Remember though that reading " + "this tome may require some time.";
        }

        public virtual void Choose(HeroSubClass way)
        {
            Detach(CurUser.Belongings.Backpack);

            CurUser.Spend(TimeToRead);
            CurUser.Busy();

            CurUser.subClass = way;

            CurUser.Sprite.DoOperate(CurUser.pos);
            Sample.Instance.Play(Assets.SND_MASTERY);

            SpellSprite.Show(CurUser, SpellSprite.Mastery);
            CurUser.Sprite.Emitter().Burst(Speck.Factory(Speck.MASTERY), 12);
            GLog.Warning("You have chosen the way of the {0}!", Utils.Capitalize(way.Title));

            if (way == HeroSubClass.BERSERKER && CurUser.HP <= CurUser.HT * Fury.Level)
                Buff.Affect<Fury>(CurUser);
        }
    }
}