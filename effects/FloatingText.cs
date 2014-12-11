using System.Collections.Generic;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.scenes;

namespace sharpdungeon.effects
{
    public class FloatingText : BitmapText
    {
        private const float Lifespan = 1f;
        private const float Distance = DungeonTilemap.Size;

        private float _timeLeft;

        private int _key = -1;

        private static readonly SparseArray<List<FloatingText>> Stacks = new SparseArray<List<FloatingText>>();

        public FloatingText()
        {
            PixelScene.ChooseFont(9);
            font = PixelScene.font;
            Scale.Set(PixelScene.scale);

            Speed.Y = -Distance / Lifespan;
        }

        public override void Update()
        {
            base.Update();

            if (!(_timeLeft > 0))
                return;

            if ((_timeLeft -= Game.Elapsed) <= 0)
                Kill();
            else
            {
                var p = _timeLeft / Lifespan;
                Alpha(p > 0.5f ? 1 : p * 2);
            }
        }

        public override void Kill()
        {
            if (_key != -1)
            {
                Stacks[_key].Remove(this);
                _key = -1;
            }
            base.Kill();
        }

        public override void Destroy()
        {
            Kill();
            base.Destroy();
        }

        public virtual void Reset(float x, float y, string text, int color)
        {
            Revive();

            Text(text);
            Hardlight(color);

            Measure();
            X = PixelScene.Align(x - Width / 2);
            Y = y - Height;

            _timeLeft = Lifespan;
        }

        public static void Show(float x, float y, string text, int color)
        {
            GameScene.Status().Reset(x, y, text, color);
        }

        public static void Show(float x, float y, int key, string text, int color)
        {
            var txt = GameScene.Status();
            txt.Reset(x, y, text, color);
            Push(txt, key);
        }

        private static void Push(FloatingText txt, int key)
        {
            txt._key = key;

            var stack = Stacks[key];
            if (stack == null)
            {
                stack = new List<FloatingText>();
                Stacks.Add(key, stack);
            }

            if (stack.Count > 0)
            {
                var below = txt;
                var aboveIndex = stack.Count - 1;
                while (aboveIndex >= 0)
                {
                    var above = stack[aboveIndex];
                    if (above.Y + above.Height > below.Y)
                    {
                        above.Y = below.Y - above.Height;

                        below = above;
                        aboveIndex--;
                    }
                    else
                        break;
                }
            }

            stack.Add(txt);
        }
    }
}