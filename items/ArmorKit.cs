using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.items.armor;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.items
{
    public class ArmorKit : Item
    {
        private const string TxtSelectArmor = "Select an armor to Upgrade";
        private const string TxtUpgraded = "you applied the armor kit to Upgrade your {0}";

        private const float TimeToUpgrade = 2;

        private const string AcApply = "APPLY";

        public ArmorKit()
        {
            name = "armor kit";
            image = ItemSpriteSheet.KIT;

            unique = true;

            _itemSelector = new ArmorKitListener(this);
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(AcApply);
            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action == AcApply)
            {
                CurUser = hero;
                GameScene.SelectItem(_itemSelector, WndBag.Mode.ARMOR, TxtSelectArmor);
            }
            else
                base.Execute(hero, action);
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override bool Identified
        {
            get { return true; }
        }

        public void Upgrade(Armor armor)
        {
            Detach(CurUser.Belongings.Backpack);

            CurUser.Sprite.CenterEmitter().Start(Speck.Factory(Speck.KIT), 0.05f, 10);
            CurUser.Spend(TimeToUpgrade);
            CurUser.Busy();

            GLog.Warning(TxtUpgraded, armor.Name);

            var classArmor = ClassArmor.Upgrade(CurUser, armor);
            if (CurUser.Belongings.Armor == armor)
            {
                CurUser.Belongings.Armor = classArmor;
                ((HeroSprite)CurUser.Sprite).UpdateArmor();
            }
            else
            {
                armor.Detach(CurUser.Belongings.Backpack);
                classArmor.Collect(CurUser.Belongings.Backpack);
            }

            CurUser.Sprite.DoOperate(CurUser.pos);
            Sample.Instance.Play(Assets.SND_EVOKE);
        }

        public override string Info()
        {
            return "Using this kit of small tools and materials anybody can transform any armor into an \"epic armor\", " + "which will keep All properties of the original armor, but will also provide its wearer a special ability " + "depending on his class. No skills in tailoring, leatherworking or blacksmithing are required.";
        }

        private readonly WndBag.Listener _itemSelector;
    }

    internal class ArmorKitListener : WndBag.Listener
    {
        private readonly ArmorKit _armorKit;

        public ArmorKitListener(ArmorKit armorKit)
        {
            _armorKit = armorKit;
        }

        public void OnSelect(Item item)
        {
            if (item != null)
                _armorKit.Upgrade((Armor)item);
        }
    }
}