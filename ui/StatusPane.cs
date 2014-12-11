using System;
using System.Globalization;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.noosa.particles;
using pdsharp.noosa.ui;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items.keys;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.windows;

namespace sharpdungeon.ui
{
    public class StatusPane : Component
    {
        private NinePatch _shield;
        private Image _avatar;
        private Emitter _blood;

        private int _lastTier;

        private Image _hp;
        private Image _exp;

        private int _lastLvl = -1;
        private int _lastKeys = -1;

        private BitmapText _level;
        private BitmapText _depth;
        private BitmapText _keys;

        private DangerIndicator _danger;
        private LootIndicator _loot;
        private BuffIndicator _buffs;
        private Compass _compass;

        private MenuButton _btnMenu;

        protected override void CreateChildren()
        {
            _shield = new NinePatch(Assets.STATUS, 80, 0, 30 + 18, 0);
            Add(_shield);
            
            var touchArea = new TouchArea(0, 1, 30, 30);
            touchArea.ClickAction = touch =>
            {
                var sprite = Dungeon.Hero.Sprite;
                if (!sprite.Visible)
                    Camera.Main.FocusOn(sprite);
                GameScene.Show(new WndHero());
            };
            Add(touchArea);

            _btnMenu = new MenuButton();
            Add(_btnMenu);

            _avatar = HeroSprite.Avatar(Dungeon.Hero.heroClass, _lastTier);
            Add(_avatar);

            _blood = new Emitter();
            _blood.Pos(_avatar);
            _blood.Pour(BloodParticle.Factory, 0.3f);
            _blood.AutoKill = false;
            _blood.On = false;
            Add(_blood);

            _compass = new Compass(Dungeon.Level.exit);
            Add(_compass);

            _hp = new Image(Assets.HP_BAR);
            Add(_hp);

            _exp = new Image(Assets.XP_BAR);
            Add(_exp);

            _level = new BitmapText(PixelScene.font1x);
            _level.Hardlight(0xFFEBA4);
            Add(_level);

            _depth = new BitmapText(Dungeon.Depth.ToString(CultureInfo.InvariantCulture), PixelScene.font1x);
            _depth.Hardlight(0xCACFC2);
            _depth.Measure();
            Add(_depth);

            Dungeon.Hero.Belongings.CountIronKeys();
            _keys = new BitmapText(PixelScene.font1x);
            _keys.Hardlight(0xCACFC2);
            Add(_keys);

            _danger = new DangerIndicator();
            Add(_danger);

            _loot = new LootIndicator();
            Add(_loot);

            _buffs = new BuffIndicator(Dungeon.Hero);
            Add(_buffs);
        }

        protected override void Layout()
        {
            _Height = 32;

            _shield.Size(Width, _shield.Height);

            _avatar.X = PixelScene.Align(Camera, _shield.X + 15 - _avatar.Width / 2);
            _avatar.Y = PixelScene.Align(Camera, _shield.Y + 16 - _avatar.Height / 2);

            _compass.X = _avatar.X + _avatar.Width / 2 - _compass.Origin.X;
            _compass.Y = _avatar.Y + _avatar.Height / 2 - _compass.Origin.Y;

            _hp.X = 30;
            _hp.Y = 3;

            _depth.X = Width - 24 - _depth.Width - 18;
            _depth.Y = 6;

            _keys.Y = 6;

            _danger.SetPos(Width - _danger.Width, 20);

            _loot.SetPos(Width - _loot.Width, _danger.Bottom() + 2);

            _buffs.SetPos(32, 11);

            _btnMenu.SetPos(Width - _btnMenu.Width, 1);
        }

        public override void Update()
        {
            base.Update();

            var health = (float)Dungeon.Hero.HP / Dungeon.Hero.HT;

            if (Math.Abs(health) < 0.0001)
            {
                _avatar.Tint(0x000000, 0.6f);
                _blood.On = false;
            }
            else
                if (health < 0.25f)
                {
                    _avatar.Tint(0xcc0000, 0.4f);
                    _blood.On = true;
                }
                else
                {
                    _avatar.ResetColor();
                    _blood.On = false;
                }

            _hp.Scale.X = health;
            _exp.Scale.X = (Width / _exp.Width) * Dungeon.Hero.Exp / Dungeon.Hero.MaxExp();

            if (Dungeon.Hero.Lvl != _lastLvl)
            {
                if (_lastLvl != -1)
                {
                    var emitter = Recycle<Emitter>();
                    emitter.Revive();
                    emitter.Pos(27, 27);
                    emitter.Burst(Speck.Factory(Speck.STAR), 12);
                }

                _lastLvl = Dungeon.Hero.Lvl;
                _level.Text(_lastLvl.ToString());
                _level.Measure();
                _level.X = PixelScene.Align(27.0f - _level.Width / 2);
                _level.Y = PixelScene.Align(27.5f - _level.BaseLine() / 2);
            }

            var k = IronKey.CurDepthQuantity;
            if (k != _lastKeys)
            {
                _lastKeys = k;
                _keys.Text(_lastKeys.ToString());
                _keys.Measure();
                _keys.X = Width - 8 - _keys.Width - 18;
            }

            var tier = Dungeon.Hero.Tier();
            if (tier == _lastTier) 
                return;

            _lastTier = tier;
            _avatar.Copy(HeroSprite.Avatar(Dungeon.Hero.heroClass, tier));
        }

        private class MenuButton : Button
        {
            private Image _image;

            public MenuButton()
            {
                _Width = _image.Width + 4;
                _Height = _image.Height + 4;
            }

            protected override void CreateChildren()
            {
                base.CreateChildren();

                _image = new Image(Assets.STATUS, 114, 3, 12, 11);
                Add(_image);
            }

            protected override void Layout()
            {
                base.Layout();

                _image.X = X + 2;
                _image.Y = Y + 2;
            }

            protected override void OnTouchDown()
            {
                _image.Brightness(1.5f);
                Sample.Instance.Play(Assets.SND_CLICK);
            }

            protected override void OnTouchUp()
            {
                _image.ResetColor();
            }

            protected override void OnClick()
            {
                GameScene.Show(new WndGame());
            }
        }
    }
}