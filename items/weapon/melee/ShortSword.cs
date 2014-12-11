using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.actors.hero;
using sharpdungeon.items.scrolls;
using sharpdungeon.items.weapon.missiles;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.items.weapon.melee
{
    public class ShortSword : MeleeWeapon
    {
        public const string AcReforge = "REFORGE";

        private const string TxtSelectWeapon = "Select a weapon to Upgrade";

        public const string TxtReforged = "you reforged the short sword to Upgrade your {0}";
        public const string TxtNotBoomerang = "you can't Upgrade a boomerang this way";

        public const float TimeToReforge = 2f;
        
        public ShortSword()
            : base(1, 1f, 1f)
        {
            name = "short sword";
            image = ItemSpriteSheet.SHORT_SWORD;
            Str = 11;
            Max = 12;
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);

            if (level > 0)
                actions.Add(AcReforge);

            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action == AcReforge)
            {
                if (hero.Belongings.Weapon == this)
                    hero.Belongings.Weapon = null;
                else
                    Detach(hero.Belongings.Backpack);

                CurUser = hero;

                GameScene.SelectItem(_itemSelector, WndBag.Mode.WEAPON, TxtSelectWeapon);
            }
            else
                base.Execute(hero, action);
        }

        public override string Desc()
        {
            return "It is indeed quite short, just a few inches longer, than a dagger.";
        }

        private readonly WndBag.Listener _itemSelector = new ShortSwordListener();
    }

    internal class ShortSwordListener : WndBag.Listener
    {
        public void OnSelect(Item item)
        {
            var sword = item as ShortSword;
            if (sword != null)
            {
                Sample.Instance.Play(Assets.SND_EVOKE);
                ScrollOfUpgrade.Upgrade(item.CurUser);
                Item.Evoke(item.CurUser);

                GLog.Warning(ShortSword.TxtReforged, item.Name);

                ((MeleeWeapon)item).SafeUpgrade();
                item.CurUser.SpendAndNext(ShortSword.TimeToReforge);

                Badge.ValidateItemLevelAquired(item);
            }
            else
            {
                if (item is Boomerang)
                    GLog.Warning(ShortSword.TxtNotBoomerang);

                if (item.IsEquipped(item.CurUser))
                    item.CurUser.Belongings.Weapon = item as Weapon;
                else
                    item.Collect(item.CurUser.Belongings.Backpack);
            }
        }
    }
}