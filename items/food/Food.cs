using System;
using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.items.scrolls;
using sharpdungeon.sprites;
using sharpdungeon.utils;

namespace sharpdungeon.items.food
{
    public class Food : Item
    {
        private const float TimeToEat = 3f;

        public const string AcEat = "EAT";

        public float Energy = Hunger.Hungry;
        public string Message = "That food tasted delicious!";

        public Food()
        {
            Stackable = true;
            name = "ration of food";
            image = ItemSpriteSheet.RATION;
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(AcEat);
            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action.Equals(AcEat))
            {
                Detach(hero.Belongings.Backpack);

                hero.Buff<Hunger>().Satisfy(Energy);
                GLog.Information(Message);

                switch (hero.heroClass.Ordinal())
                {
                    case HeroClassType.Warrior:
                        if (hero.HP < hero.HT)
                        {
                            hero.HP = Math.Min(hero.HP + 5, hero.HT);
                            hero.Sprite.Emitter().Burst(Speck.Factory(Speck.HEALING), 1);
                        }
                        break;
                    case HeroClassType.Mage:
                        hero.Belongings.Charge(false);
                        ScrollOfRecharging.Charge(hero);
                        break;
                    case HeroClassType.Rogue:
                        break;
                    case HeroClassType.Huntress:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                hero.Sprite.DoOperate(hero.pos);
                hero.Busy();
                SpellSprite.Show(hero, SpellSprite.Food);
                Sample.Instance.Play(Assets.SND_EAT);

                hero.Spend(TimeToEat);

                Statistics.FoodEaten++;
                Badge.ValidateFoodEaten();
            }
            else
                base.Execute(hero, action);
        }

        public override string Info()
        {
            return "Nothing fancy here: dried meat, " + "some biscuits - things like that.";
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override bool Identified
        {
            get { return true; }
        }

        public override int Price()
        {
            return 10 * Quantity();
        }
    }
}