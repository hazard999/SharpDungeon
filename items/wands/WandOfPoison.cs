using pdsharp.noosa.audio;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.utils;

namespace sharpdungeon.items.wands
{
    public class WandOfPoison : Wand
    {
        public WandOfPoison()
        {
            name = "Wand of Poison";
        }

        protected internal override void OnZap(int cell)
        {
            var ch = Actor.FindChar(cell);
            if (ch != null)
                Buff.Affect<Poison>(ch).Set(Poison.DurationFactor(ch) * (5 + Level));
            else
                GLog.Information("nothing happened");
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.Poison(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
        }

        public override string Desc()
        {
            return "The vile blast of this twisted bit of wood will imbue its target " + "with a deadly venom. A creature that is poisoned will suffer periodic " + "damage until the effect ends. The duration of the effect increases " + "with the Level of the staff.";
        }
    }

}