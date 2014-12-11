using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.items.wands;
using sharpdungeon.utils;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfTeleportation : Scroll
    {
        public const string TxtTeleported = "In a blink of an eye you were teleported to another location of the Level.";

        public const string TxtNoTeleport = "Strong magic aura of this place prevents you from teleporting!";

        public ScrollOfTeleportation()
        {
            name = "Scroll of Teleportation";
        }

        protected internal override void DoRead()
        {

            Sample.Instance.Play(Assets.SND_READ);
            Invisibility.Dispel();

            TeleportHero(CurUser);
            SetKnown();

            CurUser.SpendAndNext(TimeToRead);
        }

        public static void TeleportHero(Hero hero)
        {
            var count = 10;
            int pos;
            do
            {
                pos = Dungeon.Level.RandomRespawnCell();
                if (count-- <= 0)
                {
                    break;
                }

            } while (pos == -1);

            if (pos == -1)
                GLog.Warning(TxtNoTeleport);
            else
            {
                WandOfBlink.Appear(hero, pos);
                Dungeon.Level.Press(pos, hero);
                Dungeon.Observe();

                GLog.Information(TxtTeleported);
            }
        }

        public override string Desc()
        {
            return "The spell on this parchment instantly transports the reader " + "to a Random location on the dungeon Level. It can be used " + "to escape a dangerous situation, but the unlucky reader might " + "find himself in an even more dangerous place.";
        }

        public override int Price()
        {
            return IsKnown ? 40 * quantity : base.Price();
        }
    }

}