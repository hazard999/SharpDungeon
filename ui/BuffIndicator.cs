using Android.Net;
using pdsharp.gltextures;
using pdsharp.noosa;
using pdsharp.noosa.tweeners;
using pdsharp.noosa.ui;
using pdsharp.utils;
using sharpdungeon.actors;
using System.Collections.Generic;

namespace sharpdungeon.ui
{
    public class BuffIndicator : Component
    {

        public const int NONE = -1;

        public const int MIND_VISION = 0;
        public const int LEVITATION = 1;
        public const int FIRE = 2;
        public const int POISON = 3;
        public const int PARALYSIS = 4;
        public const int HUNGER = 5;
        public const int STARVATION = 6;
        public const int SLOW = 7;
        public const int OOZE = 8;
        public const int AMOK = 9;
        public const int TERROR = 10;
        public const int ROOTS = 11;
        public const int INVISIBLE = 12;
        public const int SHADOWS = 13;
        public const int WEAKNESS = 14;
        public const int FROST = 15;
        public const int BLINDNESS = 16;
        public const int COMBO = 17;
        public const int FURY = 18;
        public const int HEALING = 19;
        public const int ARMOR = 20;
        public const int HEART = 21;
        public const int LIGHT = 22;
        public const int CRIPPLE = 23;
        public const int BARKSKIN = 24;
        public const int IMMUNITY = 25;
        public const int BLEEDING = 26;
        public const int MARK = 27;
        public const int DEFERRED = 28;
        public const int VERTIGO = 29;

        public const int SIZE = 7;

        private static BuffIndicator heroInstance;

        private SmartTexture texture;
        private TextureFilm film;

        private List<Image> icons = new List<Image>();

        private Character ch;

        public BuffIndicator(Character ch)
            : base()
        {

            this.ch = ch;
            if (ch == Dungeon.Hero)
            {
                heroInstance = this;
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            if (this == heroInstance)
            {
                heroInstance = null;
            }
        }

        protected override void CreateChildren()
        {
            texture = TextureCache.Get(Assets.BUFFS_SMALL);
            film = new TextureFilm(texture, SIZE, SIZE);
        }

        protected override void Layout()
        {
            Clear();

            var newIcons = new List<Image>();

            foreach (var buff in ch.Buffs())
            {
                var icon = buff.Icon();
                if (icon == NONE)
                    continue;

                var img = new Image(texture);
                img.Frame(film.Get(icon));
                img.X = X + Members.Count * (SIZE + 2);
                img.Y = Y;
                Add(img);

                newIcons.Insert(icon, img);
            }
            for (var i = 0; i < icons.Count; i++)
            {
                if (newIcons[i] != null)
                    continue;

                var icon = icons[i];
                icon.Origin.Set(SIZE / 2);
                Add(icon);
                //Add(new AlphaTweener(icon, 0, 0.6f) { protected void updateValues(float progress) { base.updateValues(progress); image.scale.Set(1 + 5 * progress); }; });
            }

            icons = newIcons;
        }

        public static void RefreshHero()
        {
            if (heroInstance != null)
                heroInstance.Layout();
        }
    }
}