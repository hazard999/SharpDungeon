using System;
using pdsharp.noosa;
using pdsharp.utils;
using sharpdungeon.scenes;
using sharpdungeon.ui;
using System.Collections.Generic;

namespace sharpdungeon.windows
{
    public class WndStory : Window
    {
        private const int WIDTH = 120;
        private const int MARGIN = 6;

        private const float bgR = 0.77f;
        private const float bgG = 0.73f;
        private const float bgB = 0.62f;

        public const int ID_SEWERS = 0;
        public const int ID_PRISON = 1;
        public const int ID_CAVES = 2;
        public const int ID_METROPOLIS = 3;
        public const int ID_HALLS = 4;

        private static readonly List<string> CHAPTERS = new List<string>();

        static WndStory()
        {
            CHAPTERS.Insert(ID_SEWERS, "The Dungeon lies right beneath the City, its upper levels actually constitute the City's sewer system. " + "Being nominally a part of the City, these levels are not that dangerous. No one will call it a safe place, " + "but at least you won't need to deal with evil magic here.");

            CHAPTERS.Insert(ID_PRISON, "Many years ago an underground prison was built here for the most dangerous criminals. At the time it seemed " + "like a very clever idea, because this place indeed was very hard to escape. But soon dark miasma started to permeate " + "from below, driving prisoners and guards insane. In the end the prison was abandoned, though some convicts " + "were left locked up here.");

            CHAPTERS.Insert(ID_CAVES, "The caves, which stretch down under the abandoned prison, are sparcely populated. They lie too deep to be exploited " + "by the City and they are too poor in minerals to interest the dwarves. In the past there was a trade outpost " + "somewhere here on the route between these two states, but it has perished since the decline of Dwarven Metropolis. " + "Only omnipresent gnolls and subterranean animals dwell here now.");

            CHAPTERS.Insert(ID_METROPOLIS, "Dwarven Metropolis was once the greatest of dwarven city-states. In its heyday the mechanized army of dwarves " + "has successfully repelled the invasion of the old god and his demon army. But it is said, that the returning warriors " + "have brought seeds of corruption with them, and that victory was the beginning of the end for the underground kingdom.");

            CHAPTERS.Insert(ID_HALLS, "In the past these levels were the outskirts of Metropolis. After the costly victory in the war with the old god " + "dwarves were too weakened to clear them of remaining demons. Gradually demons have tightened their grip on this place " + "and now it's called Demon Halls.\\Negative\\Negative" + "Very few adventurers have ever descended this far...");
        }

        private readonly BitmapTextMultiline _tf;

        private float delay;

        public WndStory(string text)
            : base(0, 0, sharpdungeon.Chrome.Get(sharpdungeon.Chrome.Type.SCROLL))
        {
            _tf = PixelScene.CreateMultiline(text, 7);
            _tf.MaxWidth = WIDTH - MARGIN * 2;
            _tf.Measure();
            _tf.RA = bgR;
            _tf.Ga = bgG;
            _tf.Ba = bgB;
            _tf.Rm = -bgR;
            _tf.Gm = -bgG;
            _tf.Bm = -bgB;
            _tf.X = MARGIN;
            Add(_tf);

            var touchArea = new TouchArea(Chrome);
            touchArea.ClickAction = (touch) => Hide();
            
            Resize((int)(_tf.Width + MARGIN * 2), (int)Math.Min(_tf.Height, 180));
        }

        public override void Update()
        {
            base.Update();

            if (delay > 0 && (delay -= Game.Elapsed) <= 0)
                Chrome.Visible = _tf.Visible = true;
        }

        public static void ShowChapter(int id)
        {
            if (Dungeon.Chapters.Contains(id))
                return;

            var text = CHAPTERS[id];
            if (text == null)
                return;

            var wnd = new WndStory(text);
            if ((wnd.delay = 0.6f) > 0)
                wnd.Chrome.Visible = wnd._tf.Visible = false;

            Game.Scene.Add(wnd);

            Dungeon.Chapters.Add(id);
        }
    }
}