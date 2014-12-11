using pdsharp.noosa.audio;
using pdsharp.noosa.tweeners;
using pdsharp.utils;
using sharpdungeon.actors;
using sharpdungeon.effects;
using sharpdungeon.mechanics;

namespace sharpdungeon.items.wands
{
    public class WandOfBlink : Wand
    {
        public WandOfBlink()
        {
            name = "Wand of Blink";
        }

        protected internal override void OnZap(int cell)
        {
            var localLevel = Level;

            if (Ballistica.Distance > localLevel + 4)
                cell = Ballistica.Trace[localLevel + 3];
            else
                if (Actor.FindChar(cell) != null && Ballistica.Distance > 1)
                    cell = Ballistica.Trace[Ballistica.Distance - 2];

            CurUser.Sprite.Visible = true;
            Appear(Dungeon.Hero, cell);
            Dungeon.Observe();
        }

        protected internal override void Fx(int cell, ICallback callback)
        {
            MagicMissile.WhiteLight(CurUser.Sprite.Parent, CurUser.pos, cell, callback);
            Sample.Instance.Play(Assets.SND_ZAP);
            CurUser.Sprite.Visible = false;
        }

        public static void Appear(Character ch, int pos)
        {
            ch.Sprite.InterruptMotion();

            ch.Move(pos);
            ch.Sprite.Place(pos);

            if (ch.invisible == 0)
            {
                ch.Sprite.Alpha(0);
                ch.Sprite.Parent.Add(new AlphaTweener(ch.Sprite, 1, 0.4f));
            }

            ch.Sprite.Emitter().Start(Speck.Factory(Speck.LIGHT), 0.2f, 3);
            Sample.Instance.Play(Assets.SND_TELEPORT);
        }

        public override string Desc()
        {
            return "This wand will allow you to teleport in the chosen direction. " + "Creatures and inanimate obstructions will block the teleportation.";
        }
    }
}