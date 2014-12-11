using pdsharp.noosa;
using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.actors.mobs;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;

namespace sharpdungeon.levels.features
{
    public class Chasm 
    {
        private const string TXT_CHASM = "Chasm";
        private const string TXT_YES = "Yes, I know what I'm doing";
        private const string TXT_NO = "No, I changed my mind";
        private const string TXT_JUMP = "Do you really want to jump into the chasm? You can probably DoDie.";

        public static bool JumpConfirmed;

        public static void HeroJump(Hero hero)
        {
            var wnd = new WndOptions(TXT_CHASM, TXT_JUMP, TXT_YES, TXT_NO);

            wnd.SelectAction = (index) =>
            {
                if (index != 0) 
                    return;

                JumpConfirmed = true;
                hero.Resume();
            };

            GameScene.Show(wnd);
        }

        public static void HeroFall(int pos)
        {
            JumpConfirmed = false;

            Sample.Instance.Play(Assets.SND_FALLING);

            if (Dungeon.Hero.IsAlive)
            {
                Dungeon.Hero.Interrupt();
                InterlevelScene.mode = InterlevelScene.Mode.FALL;
                var level = Dungeon.Level as RegularLevel;
                if (level != null)
                {
                    var room = level.Room(pos);
                    InterlevelScene.fallIntoPit = room != null && room.type == RoomType.WEAK_FLOOR;
                }
                else
                    InterlevelScene.fallIntoPit = false;

                Game.SwitchScene<InterlevelScene>();
            }
            else
                Dungeon.Hero.Sprite.Visible = false;
        }

        public static void HeroLand()
        {
            var hero = Dungeon.Hero;

            hero.Sprite.Burst(hero.Sprite.Blood(), 10);
            Camera.Main.Shake(4, 0.2f);

            Buff.Prolong<Cripple>(hero, Cripple.Duration);

            hero.Damage(Random.IntRange(hero.HT / 3, hero.HT / 2), new ChasmDoom());
        }

        public static void MobFall(Mob mob)
        {
            // Destroy instead of kill to prevent dropping loot
            mob.Destroy();

            ((MobSprite)mob.Sprite).Fall();
        }

    }

    public class ChasmDoom : Hero.IDoom
    {
        public void OnDeath()
        {
            Badge.ValidateDeathFromFalling();
            Dungeon.Fail(Utils.Format(ResultDescriptions.FALL, Dungeon.Depth));
            GLog.Negative("You fell to death...");
        }        
    }
}