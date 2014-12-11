using System.Collections.Generic;
using sharpdungeon.actors.mobs;
using sharpdungeon.levels;
using sharpdungeon.sprites;
using sharpdungeon.scenes;
using System;

namespace sharpdungeon.ui
{
    public class AttackIndicator : Tag
    {
        private const float Enabled = 1.0f;
        private const float Disabled = 0.3f;

        private static AttackIndicator _instance;

        private CharSprite _sprite;

        private static Mob _lastTarget;
        private readonly List<Mob> _candidates = new List<Mob>();

        public AttackIndicator()
            : base(DangerIndicator.COLOR)
        {
            _instance = this;

            SetSize(24, 24);
            Visible(false);
            Enable(false);
        }

        protected override void Layout()
        {
            base.Layout();

            if (_sprite == null)
                return;

            _sprite.X = X + (Width - _sprite.Width) / 2;
            _sprite.Y = Y + (Height - _sprite.Height) / 2;
            PixelScene.Align(_sprite);
        }

        public override void Update()
        {
            base.Update();

            if (Dungeon.Hero.IsAlive)
            {
                if (!Dungeon.Hero.ready)
                    Enable(false);
            }
            else
            {
                Visible(false);
                Enable(false);
            }
        }

        private void CheckEnemies()
        {
            var heroPos = Dungeon.Hero.pos;
            _candidates.Clear();
            var v = Dungeon.Hero.VisibleEnemies;
            for (var i = 0; i < v; i++)
            {
                var mob = Dungeon.Hero.VisibleEnemy(i);
                if (Level.Adjacent(heroPos, mob.pos))
                    _candidates.Add(mob);
            }

            if (!_candidates.Contains(_lastTarget))
            {
                if (_candidates.Count == 0)
                    _lastTarget = null;
                else
                {
                    _lastTarget = pdsharp.utils.Random.Element(_candidates);
                    UpdateImage();
                    Flash();
                }
            }
            else
            {
                if (!Bg.Visible)
                    Flash();
            }

            Visible(_lastTarget != null);
            Enable(Bg.Visible);
        }

        private void UpdateImage()
        {
            if (_sprite != null)
            {
                _sprite.KillAndErase();
                _sprite = null;
            }

            try
            {
                _sprite = (CharSprite)Activator.CreateInstance(_lastTarget.SpriteClass);
                _sprite.Idle();
                _sprite.paused = true;
                Add(_sprite);

                _sprite.X = X + (Width - _sprite.Width) / 2 + 1;
                _sprite.Y = Y + (Height - _sprite.Height) / 2;
                PixelScene.Align(_sprite);
            }
            catch (Exception)
            {
            }
        }

        private bool _enabled = true;
        private void Enable(bool value)
        {
            _enabled = value;
            if (_sprite != null)
                _sprite.Alpha(value ? Enabled : Disabled);
        }

        private new void Visible(bool value)
        {
            Bg.Visible = value;
            if (_sprite != null)
                _sprite.Visible = value;
        }

        protected override void OnClick()
        {
            if (_enabled)
                Dungeon.Hero.Handle(_lastTarget.pos);
        }

        public static void Target(actors.Character target)
        {
            _lastTarget = (Mob)target;
            _instance.UpdateImage();

            HealthIndicator.Instance.Target(target);
        }

        public static void UpdateState()
        {
            _instance.CheckEnemies();
        }
    }
}