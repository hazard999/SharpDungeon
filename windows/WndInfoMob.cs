using System;
using System.Text;
using Android.Graphics;
using pdsharp.noosa;
using pdsharp.noosa.ui;
using sharpdungeon.actors.mobs;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.ui;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndInfoMob : WndTitledMessage
    {
        public WndInfoMob(Mob mob) : base(new MobTitle(mob), Desc(mob)) { }

        private static string Desc(Mob mob)
        {
            var builder = new StringBuilder(mob.Description());

            builder.Append("\\Negative\\Negative" + mob.State.Status() + ".");

            return builder.ToString();
        }

        private class MobTitle : Component
        {
            private readonly Color _colorBg = Color.Argb(0xFF, 0xCC, 0x00, 0x00);
            private readonly Color _colorLvl = Color.Argb(0xFF, 0x00, 0xEE, 0x00);

            private const int BarHeight = 2;
            private const int Gap = 2;

            private readonly CharSprite _image;
            private readonly BitmapText _name;
            private readonly ColorBlock _hpBg;
            private readonly ColorBlock _hpLvl;
            private readonly BuffIndicator _buffs;

            private readonly float _hp;

            public MobTitle(Mob mob)
            {

                _hp = (float)mob.HP / mob.HT;

                _name = PixelScene.CreateText(Utils.Capitalize(mob.Name), 9);
                _name.Hardlight(TitleColor);
                _name.Measure();
                Add(_name);

                _image = mob.Sprite;
                Add(_image);

                _hpBg = new ColorBlock(1, 1, _colorBg);
                Add(_hpBg);

                _hpLvl = new ColorBlock(1, 1, _colorLvl);
                Add(_hpLvl);

                _buffs = new BuffIndicator(mob);
                Add(_buffs);
            }

            protected override void Layout()
            {
                _image.X = 0;
                _image.Y = Math.Max(0, _name.Height + Gap + BarHeight - _image.Height);

                _name.X = _image.Width + Gap;
                _name.Y = _image.Height - BarHeight - Gap - _name.BaseLine();

                var w = Width - _image.Width - Gap;

                _hpBg.Size(w, BarHeight);
                _hpLvl.Size(w * _hp, BarHeight);

                _hpBg.X = _hpLvl.X = _image.Width + Gap;
                _hpBg.Y = _hpLvl.Y = _image.Height - BarHeight;

                _buffs.SetPos(_name.X + _name.Width + Gap, _name.Y + _name.BaseLine() - BuffIndicator.SIZE);

                _Height = _hpBg.Y + _hpBg.Height;
            }
        }
    }
}