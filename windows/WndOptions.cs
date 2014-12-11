using System;
using sharpdungeon.scenes;
using sharpdungeon.ui;

namespace sharpdungeon.windows
{
    public class WndOptions : Window
    {
        private const int WIDTH = 120;
        private const int Margin = 2;
        private const int ButtonHeight = 20;

        public WndOptions(string title, string message, params string[] options)
        {
            var tfTitle = PixelScene.CreateMultiline(title, 9);
            tfTitle.Hardlight(TitleColor);
            tfTitle.X = tfTitle.Y = Margin;
            tfTitle.MaxWidth = WIDTH - Margin * 2;
            tfTitle.Measure();
            Add(tfTitle);

            var tfMesage = PixelScene.CreateMultiline(message, 8);
            tfMesage.MaxWidth = WIDTH - Margin * 2;
            tfMesage.Measure();
            tfMesage.X = Margin;
            tfMesage.Y = tfTitle.Y + tfTitle.Height + Margin;
            Add(tfMesage);

            var pos = tfMesage.Y + tfMesage.Height + Margin;
            var i = 0;
            foreach (var option in options)
            {
                var btn = new RedButton(option);
                btn.Index = 1;
                btn.ClickAction = (button) =>
                {
                    Hide();
                    OnSelect(button.Index);
                };
                btn.SetRect(Margin, pos, WIDTH - Margin * 2, ButtonHeight);
                Add(btn);

                pos += ButtonHeight + Margin;
                i++;
            }

            Resize(WIDTH, (int)pos);
        }

        public Action<int> SelectAction;

        protected internal virtual void OnSelect(int index)
        {
            if (SelectAction != null)
                SelectAction(index);
        }
    }
}