using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.effects;
using sharpdungeon.levels;
using sharpdungeon.scenes;
using sharpdungeon.utils;

namespace sharpdungeon.items.scrolls
{
	public class ScrollOfMagicMapping : Scroll
	{
		private const string TxtLayout = "You are now aware of the Level layout.";

	    public ScrollOfMagicMapping()
	    {
            name = "Scroll of Magic Mapping";
	    }

		protected internal override void DoRead()
		{
			const int length = Level.Length;
			var map = Dungeon.Level.map;
			var mapped = Dungeon.Level.mapped;
			var discoverable = Level.discoverable;

			var noticed = false;

			for (var i=0; i < length; i++)
			{
				var terr = map[i];

			    if (!discoverable[i]) 
                    continue;

			    mapped[i] = true;
			    if ((Terrain.Flags[terr] & Terrain.SECRET) == 0) 
                    continue;

			    Level.Set(i, Terrain.discover(terr));
			    GameScene.UpdateMap(i);

			    if (!Dungeon.Visible[i]) 
                    continue;

			    GameScene.DiscoverTile(i, terr);
			    Discover(i);

			    noticed = true;
			}

			Dungeon.Observe();

			GLog.Information(TxtLayout);
		    if (noticed)
		        Sample.Instance.Play(Assets.SND_SECRET);

		    SpellSprite.Show(CurUser, SpellSprite.Map);
			Sample.Instance.Play(Assets.SND_READ);
			Invisibility.Dispel();

			SetKnown();

			CurUser.SpendAndNext(TimeToRead);
		}

		public override string Desc()
		{
			return "When this scroll is read, an Image of crystal clarity will be etched into your memory, " + "alerting you to the precise layout of the level and revealing All hidden secrets. " + "The locations of items and creatures will remain unknown.";
		}

		public override int Price()
		{
			return IsKnown ? 25 * quantity : base.Price();
		}

		public static void Discover(int cell)
		{
			CellEmitter.Get(cell).Start(Speck.Factory(Speck.DISCOVER), 0.1f, 4);
		}
	}
}