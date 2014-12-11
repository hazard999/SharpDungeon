using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.utils;

namespace sharpdungeon.items.wands
{
    public class WandOfAmok : Wand
    {
        public WandOfAmok()
        {
            name = "Wand of Amok";
        }

        protected internal override void OnZap(int cell)
        {
            var ch = Actor.FindChar(cell);
            if (ch != null)
            {
                if (ch == Dungeon.Hero)
                    Buff.Affect<Vertigo>(ch, Vertigo.Duration(ch));
                else
                    Buff.Affect<Amok>(ch, 3f + Level);
            }
            else
                GLog.Information("nothing happened");
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.PurpleLight(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        public override string Desc()
        {
            return "The purple light from this wand will make the target run amok " + "attacking Random creatures in its vicinity.";
        }
    }
}