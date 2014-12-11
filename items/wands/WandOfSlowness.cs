using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.utils;

namespace sharpdungeon.items.wands
{
    public class WandOfSlowness : Wand
    {
        public WandOfSlowness()
        {
            name = "Wand of Slowness";
        }
        
        protected internal override void OnZap(int cell)
        {
            var ch = Actor.FindChar(cell);
            if (ch != null)
                Buff.Affect<Slow>(ch, Slow.Duration(ch) / 3 + Level);
            else
                GLog.Information("nothing happened");
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.Slowness(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        public override string Desc()
        {
            return "This wand will cause a creature to move and DoAttack " + "at half its ordinary speed until the effect ends";
        }
    }
}