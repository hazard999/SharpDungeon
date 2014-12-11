using System.Collections.Generic;
using sharpdungeon.scenes;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndChallenges : Window
    {
        private const int WIDTH = 108;
        private const int BtnHeight = 20;
        private const int Gap = 2;

        private const string Title = "Challenges";

        private readonly bool _editable;
        private readonly List<CheckBox> _boxes;

        public WndChallenges(int isChecked, bool editable)
        {
            _editable = editable;

            var title = PixelScene.CreateText(Title, 9);
            title.Hardlight(TitleColor);
            title.Measure();
            title.X = PixelScene.Align(Camera, (WIDTH - title.Width) / 2);
            Add(title);

            _boxes = new List<CheckBox>();

            var pos = title.Height + Gap;
            for (var i=0; i < Challenges.NAMES.Length; i++)
            {
                var cb = new CheckBox(Challenges.NAMES[i]);
                cb.SetChecked((isChecked & Challenges.MASKS[i]) != 0);
                cb.Active = editable;

                if (i > 0)
                    pos += Gap;

                cb.SetRect(0, pos, WIDTH, BtnHeight);
                pos = cb.Bottom();

                Add(cb);
                _boxes.Add(cb);
            }

            Resize(WIDTH, (int)pos);
        }

        public override void OnBackPressed()
        {
            if (_editable)
            {
                var value = 0;
                
                for (var i = 0; i < _boxes.Count; i++)
                    if (_boxes[i].Checked())
                        value |= Challenges.MASKS[i];

                PixelDungeon.Challenges(value);
            }

            base.OnBackPressed();
        }
    }
}