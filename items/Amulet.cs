using System.Collections.Generic;
using pdsharp.noosa;
using sharpdungeon.actors.hero;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using System.IO;

namespace sharpdungeon.items
{
    public class Amulet : Item
    {
        private const string AcEnd = "END THE GAME";

        public Amulet()
        {
            name = "Amulet of Yendor";
            image = ItemSpriteSheet.AMULET;

            unique = true;
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(AcEnd);
            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action == AcEnd)
                ShowAmuletScene(false);
            else
                base.Execute(hero, action);
        }

        public override bool DoPickUp(Hero hero)
        {
            if (!base.DoPickUp(hero))
                return false;

            if (Statistics.AmuletObtained)
                return true;

            Statistics.AmuletObtained = true;
            Badge.ValidateVictory();

            ShowAmuletScene(true);

            return true;
        }

        private void ShowAmuletScene(bool showText)
        {
            try
            {
                Dungeon.SaveAll();
                AmuletScene.NoText = !showText;
                Game.SwitchScene<AmuletScene>();
            }
            catch (IOException)
            {
            }
        }

        public override bool Identified
        {
            get { return true; }
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override string Info()
        {
            return "The Amulet of Yendor is the most powerful known artifact of unknown origin. It is said that the amulet " + "is able to fulfil any wish if its owner's will-power is strong enough to \"persuade\" it to do it.";
        }
    }
}