using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.utils;

namespace sharpdungeon.items.scrolls
{
    public class ScrollOfChallenge : Scroll
	{
        public ScrollOfChallenge()
        {
            name = "Scroll of Challenge";
        }

		protected internal override void DoRead()
		{
		    foreach (var mob in Dungeon.Level.mobs)
		        mob.Beckon(CurUser.pos);

		    GLog.Warning("The scroll emits a challenging roar that echoes throughout the dungeon!");
			SetKnown();

			CurUser.Sprite.CenterEmitter().Start(Speck.Factory(Speck.SCREAM), 0.3f, 3);
			Sample.Instance.Play(Assets.SND_CHALLENGE);
			Invisibility.Dispel();

			CurUser.SpendAndNext(TimeToRead);
		}

		public override string Desc()
		{
			return "When read aloud, this scroll will unleash a challenging roar " + "that will awaken All monsters and alert them to the reader's location.";
		}
	}
}