using System;
using System.IO;
using pdsharp.noosa;
using pdsharp.noosa.audio;
using sharpdungeon.actors;
using sharpdungeon.items;
using sharpdungeon.levels;
using sharpdungeon.windows;

namespace sharpdungeon.scenes
{
    public class InterlevelScene : PixelScene
    {
        private const float TIME_TO_FADE = 0.3f;

        private const string TXT_DESCENDING = "Descending...";
        private const string TXT_ASCENDING = "Ascending...";
        private const string TXT_LOADING = "Loading...";
        private const string TXT_RESURRECTING = "Resurrecting...";
        private const string TXT_RETURNING = "Returning...";
        private const string TXT_FALLING = "Falling...";

        private const string ERR_FILE_NOT_FOUND = "File not found. For some reason.";
        private const string ERR_GENERIC = "Something went wrong...";

        public enum Mode
        {
            DESCEND,
            ASCEND,
            CONTINUE,
            RESURRECT,
            RETURN,
            FALL
        }
        public static Mode mode;

        public static int returnDepth;
        public static int returnPos;

        public static bool noStory;

        public static bool fallIntoPit;

        private enum Phase
        {
            FADE_IN,
            STATIC,
            FADE_OUT
        }
        private Phase phase;
        private float timeLeft;

        private BitmapText message;

        //private Thread thread;
        private string error;

        public override void Create()
        {
            base.Create();

            string text = "";
            switch (mode)
            {
                case Mode.DESCEND:
                    text = TXT_DESCENDING;
                    break;
                case Mode.ASCEND:
                    text = TXT_ASCENDING;
                    break;
                case Mode.CONTINUE:
                    text = TXT_LOADING;
                    break;
                case Mode.RESURRECT:
                    text = TXT_RESURRECTING;
                    break;
                case Mode.RETURN:
                    text = TXT_RETURNING;
                    break;
                case Mode.FALL:
                    text = TXT_FALLING;
                    break;
            }

            message = CreateText(text, 9);
            message.Measure();
            message.X = (Camera.Main.CameraWidth - message.Width) / 2;
            message.Y = (Camera.Main.CameraHeight - message.Height) / 2;
            Add(message);

            phase = Phase.FADE_IN;
            timeLeft = TIME_TO_FADE;
            Run();

            //thread = new Thread(Run);
            //thread.Start();
        }

        private void Run()
        {
            try
            {
                Generator.Reset();

                Sample.Instance.Load(Assets.SND_OPEN, Assets.SND_UNLOCK, Assets.SND_ITEM, Assets.SND_DEWDROP, Assets.SND_HIT, Assets.SND_MISS, Assets.SND_STEP, Assets.SND_WATER, Assets.SND_DESCEND, Assets.SND_EAT, Assets.SND_READ, Assets.SND_LULLABY, Assets.SND_DRINK, Assets.SND_SHATTER, Assets.SND_ZAP, Assets.SND_LIGHTNING, Assets.SND_LEVELUP, Assets.SND_DEATH, Assets.SND_CHALLENGE, Assets.SND_CURSED, Assets.SND_EVOKE, Assets.SND_TRAP, Assets.SND_TOMB, Assets.SND_ALERT, Assets.SND_MELD, Assets.SND_BOSS, Assets.SND_BLAST, Assets.SND_PLANT, Assets.SND_RAY, Assets.SND_BEACON, Assets.SND_TELEPORT, Assets.SND_CHARMS, Assets.SND_MASTERY, Assets.SND_PUFF, Assets.SND_ROCKS, Assets.SND_BURNING, Assets.SND_FALLING, Assets.SND_GHOST, Assets.SND_SECRET, Assets.SND_BONES);

                switch (mode)
                {
                    case Mode.DESCEND:
                        Descend();
                        break;
                    case Mode.ASCEND:
                        Ascend();
                        break;
                    case Mode.CONTINUE:
                        Restore();
                        break;
                    case Mode.RESURRECT:
                        Resurrect();
                        break;
                    case Mode.RETURN:
                        ReturnTo();
                        break;
                    case Mode.FALL:
                        Fall();
                        break;
                }

                if ((Dungeon.Depth % 5) == 0)
                    Sample.Instance.Load(Assets.SND_BOSS);
            }
            catch (FileNotFoundException)
            {
                error = ERR_FILE_NOT_FOUND;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                // ERR_GENERIC;
            }

            if (phase != Phase.STATIC || error != null)
                return;

            phase = Phase.FADE_OUT;
            timeLeft = TIME_TO_FADE;
        }

        public override void Update()
        {
            base.Update();

            var p = timeLeft / TIME_TO_FADE;

            switch (phase)
            {

                case Phase.FADE_IN:
                    message.Alpha(1 - p);
                    if ((timeLeft -= Game.Elapsed) <= 0)
                    {
                        if (/*!thread.IsAlive &&*/error == null)
                        {
                            phase = Phase.FADE_OUT;
                            timeLeft = TIME_TO_FADE;
                        }
                        else
                            phase = Phase.STATIC;
                    }
                    break;

                case Phase.FADE_OUT:
                    message.Alpha(p);
                    if (mode == Mode.CONTINUE || (mode == Mode.DESCEND && Dungeon.Depth == 1))
                        Music.Instance.Volume(p);

                    if ((timeLeft -= Game.Elapsed) <= 0)
                        Game.SwitchScene<GameScene>();
                    break;

                case Phase.STATIC:
                    if (error != null)
                    {
                        var errorWnd = new WndError(error);
                        errorWnd.BackPressedAction += Game.SwitchScene<StartScene>;
                        error = null;
                    }
                    break;
            }
        }

        private void Descend()
        {
            Actor.FixTime();
            if (Dungeon.Hero == null)
            {
                Dungeon.Init();
                if (noStory)
                {
                    Dungeon.Chapters.Add(WndStory.ID_SEWERS);
                    noStory = false;
                }
            }
            else
                Dungeon.SaveLevel();

            Level level;
            if (Dungeon.Depth >= Statistics.DeepestFloor)
                level = Dungeon.NewLevel();
            else
            {
                Dungeon.Depth++;
                level = Dungeon.LoadLevel(Dungeon.Hero.heroClass);
            }
            Dungeon.SwitchLevel(level, level.entrance);
        }

        private void Fall()
        {
            Actor.FixTime();
            Dungeon.SaveLevel();

            Level level;
            if (Dungeon.Depth >= Statistics.DeepestFloor)
                level = Dungeon.NewLevel();
            else
            {
                Dungeon.Depth++;
                level = Dungeon.LoadLevel(Dungeon.Hero.heroClass);
            }
            Dungeon.SwitchLevel(level, fallIntoPit ? level.PitCell() : level.RandomRespawnCell());
        }

        private void Ascend()
        {
            Actor.FixTime();

            Dungeon.SaveLevel();
            Dungeon.Depth--;
            var level = Dungeon.LoadLevel(Dungeon.Hero.heroClass);
            Dungeon.SwitchLevel(level, level.exit);
        }

        private void ReturnTo()
        {
            Actor.FixTime();

            Dungeon.SaveLevel();
            Dungeon.Depth = returnDepth;
            var level = Dungeon.LoadLevel(Dungeon.Hero.heroClass);
            Dungeon.SwitchLevel(level, Level.resizingNeeded ? level.AdjustPos(returnPos) : returnPos);
        }

        private void Restore()
        {
            Actor.FixTime();

            Dungeon.LoadGame(StartScene.curClass);
            if (Dungeon.Depth == -1)
            {
                Dungeon.Depth = Statistics.DeepestFloor;
                Dungeon.SwitchLevel(Dungeon.LoadLevel(StartScene.curClass), -1);
            }
            else
            {
                var level = Dungeon.LoadLevel(StartScene.curClass);
                Dungeon.SwitchLevel(level, Level.resizingNeeded ? level.AdjustPos(Dungeon.Hero.pos) : Dungeon.Hero.pos);
            }
        }

        private void Resurrect()
        {
            Actor.FixTime();

            if (Dungeon.BossLevel())
            {
                Dungeon.Hero.Resurrect(Dungeon.Depth);
                Dungeon.Depth--;
                var level = Dungeon.NewLevel(); // true
                Dungeon.SwitchLevel(level, level.entrance);
            }
            else
            {
                Dungeon.Hero.Resurrect(-1);
                Dungeon.ResetLevel();
            }
        }

        protected override void OnBackPressed()
        {
        }
    }
}