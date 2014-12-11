using pdsharp.noosa.audio;
using pdsharp.noosa.tweeners;
using sharpdungeon.actors;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.utils;

namespace sharpdungeon.items.potions
{
	public class PotionOfInvisibility : Potion
	{
		private const float Alpha = 0.4f;

	    public PotionOfInvisibility()
	    {
            name = "Potion of Invisibility";
	    }

		protected internal override void Apply(Hero hero)
		{
			SetKnown();
			Buff.Affect<Invisibility>(hero, Invisibility.Duration);
			GLog.Information("You see your hands turn invisible!");
			Sample.Instance.Play(Assets.SND_MELD);
		}

		public override string Desc()
		{
			return "Drinking this potion will render you temporarily invisible. While invisible, " + "enemies will be unable to see you. Attacking an enemy, as well as using a wand or a scroll " + "before enemy's eyes, will dispel the effect.";
		}

		public override int Price()
		{
			return IsKnown ? 40 * Quantity() : base.Price();
		}

		public static void Melt(Character ch)
		{
		    if (ch.Sprite.Parent != null)
		        ch.Sprite.Parent.Add(new AlphaTweener(ch.Sprite, Alpha, 0.4f));
		    else
		        ch.Sprite.Alpha(Alpha);
		}
	}
}