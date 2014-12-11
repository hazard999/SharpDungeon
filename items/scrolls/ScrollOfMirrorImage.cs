using System.Linq;
using pdsharp.noosa.audio;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.mobs.npcs;
using sharpdungeon.items.wands;
using sharpdungeon.levels;
using sharpdungeon.scenes;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfMirrorImage : Scroll
    {
        private const int Nimages = 3;

        public ScrollOfMirrorImage()
        {
            name = "Scroll of Mirror Image";
        }

        protected internal override void DoRead()
        {
            var respawnPoints = Level.NEIGHBOURS8.Select(t => CurUser.pos + t).Where(p => Actor.FindChar(p) == null && (Level.passable[p] || Level.avoid[p])).ToList();

            var nImages = Nimages;
            while (nImages > 0 && respawnPoints.Count > 0)
            {
                var index = pdsharp.utils.Random.Index(respawnPoints);

                var mob = new MirrorImage();
                mob.Duplicate(CurUser);
                GameScene.Add(mob);
                WandOfBlink.Appear(mob, respawnPoints[index]);

                respawnPoints.Remove(index);
                nImages--;
            }

            if (nImages < Nimages)
                SetKnown();

            Sample.Instance.Play(Assets.SND_READ);
            Invisibility.Dispel();

            CurUser.SpendAndNext(TimeToRead);
        }

        public override string Desc()
        {
            return "The incantation on this scroll will create illusionary twins of the reader, which will chase his enemies.";
        }
    }
}