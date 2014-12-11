using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.effects;
using sharpdungeon.items.scrolls;
using sharpdungeon.utils;

namespace sharpdungeon.items.wands
{
    public class WandOfTeleportation : Wand
    {
        public WandOfTeleportation()
        {
            name = "Wand of Teleportation";
        }

        protected internal override void OnZap(int cell)
        {
            var ch = Actor.FindChar(cell);

            if (ch == CurUser)
            {
                SetKnown();
                ScrollOfTeleportation.TeleportHero(CurUser);

            }
            else
                if (ch != null)
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
                        GLog.Warning(ScrollOfTeleportation.TxtNoTeleport);
                    else
                    {
                        ch.pos = pos;
                        ch.Sprite.Place(ch.pos);
                        ch.Sprite.Visible = Dungeon.Visible[pos];
                        GLog.Information(CurUser.Name + " teleported " + ch.Name + " to somewhere");
                    }
                }
                else
                    GLog.Information("nothing happened");
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.ColdLight(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        public override string Desc()
        {
            return "A blast from this wand will teleport a creature against " + "its will to a Random place on the current Level.";
        }
    }
}