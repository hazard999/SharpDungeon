using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.actors.hero;
using sharpdungeon.items.weapon;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.items
{
    public class Weightstone : Item
    {
        private const string TxtSelectWeapon = "Select a weapon to balance";
        private const string TxtFast = "you balanced your {0} to make it faster";
        private const string TxtAccurate = "you balanced your {0} to make it more accurate";

        private const float TimeToApply = 2;

        private const string AcApply = "APPLY";

        public Weightstone()
        {
            name = "weightstone";
            image = ItemSpriteSheet.WEIGHT;

            Stackable = true;
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
                GameScene.SelectItem(_itemSelector, WndBag.Mode.WEAPON, TxtSelectWeapon);
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

        public static void Apply(Weapon weapon, bool forSpeed)
        {
            weapon.Detach(weapon.CurUser.Belongings.Backpack);

            if (forSpeed)
            {
                weapon.imbue = Weapon.Imbue.Speed;
                GLog.Positive(TxtFast, weapon.Name);
            }
            else
            {
                weapon.imbue = Weapon.Imbue.Accuracy;
                GLog.Positive(TxtAccurate, weapon.Name);
            }

            weapon.CurUser.Sprite.DoOperate(weapon.CurUser.pos);
            Sample.Instance.Play(Assets.SND_MISS);

            weapon.CurUser.Spend(TimeToApply);
            weapon.CurUser.Busy();
        }

        public override int Price()
        {
            return 40 * Quantity();
        }

        public override string Info()
        {
            return "Using a weightstone, you can balance your melee weapon to increase its speed or accuracy.";
        }

        private readonly WndBag.Listener _itemSelector = new WeightstoneListener();


        public class WndBalance : Window
        {
            private const string TXT_CHOICE = "How would you like to balance your {0}?";

            private const string TXT_SPEED = "For speed";
            private const string TXT_ACCURACY = "For accuracy";
            private const string TXT_CANCEL = "Never mind";

            private const int WIDTH = 120;
            private const int MARGIN = 2;
            private const int BUTTON_WIDTH = WIDTH - MARGIN * 2;
            private const int BUTTON_HEIGHT = 20;

            public WndBalance(Weapon weapon)
            {
                var titlebar = new IconTitle(weapon);
                titlebar.SetRect(0, 0, WIDTH, 0);
                Add(titlebar);

                var tfMesage = PixelScene.CreateMultiline(Utils.Format(TXT_CHOICE, weapon.Name), 8);
                tfMesage.MaxWidth = WIDTH - MARGIN * 2;
                tfMesage.Measure();
                tfMesage.X = MARGIN;
                tfMesage.Y = titlebar.Bottom() + MARGIN;
                Add(tfMesage);

                var pos = tfMesage.Y + tfMesage.Height;

                if (weapon.imbue != Weapon.Imbue.Speed)
                {
                    var btnSpeed = new RedButton(TXT_SPEED);
                    btnSpeed.ClickAction = button =>
                    {
                        Hide();
                        Apply(weapon, true);
                    };
                    btnSpeed.SetRect(MARGIN, pos + MARGIN, BUTTON_WIDTH, BUTTON_HEIGHT);
                    Add(btnSpeed);

                    pos = btnSpeed.Bottom();
                }

                if (weapon.imbue != Weapon.Imbue.Accuracy)
                {
                    var btnAccuracy = new RedButton(TXT_ACCURACY);
                    btnAccuracy.ClickAction = button =>
                    {
                        Hide();
                        Apply(weapon, false);
                    };
                    btnAccuracy.SetRect(MARGIN, pos + MARGIN, BUTTON_WIDTH, BUTTON_HEIGHT);
                    Add(btnAccuracy);

                    pos = btnAccuracy.Bottom();
                }

                var btnCancel = new RedButton(TXT_CANCEL);
                btnCancel.ClickAction = button => Hide();
                btnCancel.SetRect(MARGIN, pos + MARGIN, BUTTON_WIDTH, BUTTON_HEIGHT);
                Add(btnCancel);

                Resize(WIDTH, (int)btnCancel.Bottom() + MARGIN);
            }

            protected internal virtual void OnSelect(int index)
            {
            }
        }
    }

    internal class WeightstoneListener : WndBag.Listener
    {
        public void OnSelect(Item item)
        {
            if (item != null)
                GameScene.Show(new Weightstone.WndBalance((Weapon)item));
        }
    }
}