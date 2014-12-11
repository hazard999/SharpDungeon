using System.Globalization;
using pdsharp.noosa;
using sharpdungeon.actors.mobs;
using sharpdungeon.scenes;

namespace sharpdungeon.ui
{
    public class DangerIndicator : Tag
    {
        public const int COLOR = 0xFF4C4C;

        private BitmapText number;
        private Image icon;

        private int enemyIndex = 0;

        private int lastNumber = -1;

        public DangerIndicator()
            : base(0xFF4C4C)
        {
            SetSize(24, 16);

            Visible = false;
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            number = new BitmapText(PixelScene.font1x);
            Add(number);

            icon = Icons.SKULL.Get();
            Add(icon);
        }

        protected override void Layout()
        {
            base.Layout();

            icon.X = Right() - 10;
            icon.Y = Y + (Height - icon.Height) / 2;

            PlaceNumber();
        }

        private void PlaceNumber()
        {
            number.X = Right() - 11 - number.Width;
            number.Y = PixelScene.Align(Y + (Height - number.BaseLine()) / 2);
        }

        public override void Update()
        {
            if (Dungeon.Hero.IsAlive)
            {
                var v = Dungeon.Hero.VisibleEnemies;
                if (v != lastNumber)
                {
                    lastNumber = v;
                    Visible = lastNumber > 0;
                    if (Visible)
                    {
                        number.Text(lastNumber.ToString(CultureInfo.InvariantCulture));
                        number.Measure();
                        PlaceNumber();

                        Flash();
                    }
                }
            }
            else
                Visible = false;

            base.Update();
        }

        protected override void OnClick()
        {
            var target = Dungeon.Hero.VisibleEnemy(enemyIndex++);

            HealthIndicator.Instance.Target(target == HealthIndicator.Instance.Target() ? null : target);

            Camera.Main.Target = null;
            Camera.Main.FocusOn(target.Sprite);
        }
    }
}