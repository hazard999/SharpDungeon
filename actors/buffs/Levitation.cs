using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class Levitation : FlavourBuff
	{
		public const float Duration = 20f;

		public override bool AttachTo(Character target)
		{
		    if (base.AttachTo(target))
			{
				target.Flying = true;
				Buff.Detach<Roots>(target);
				return true;
			}
		    
            return false;
		}

	    public override void Detach()
		{
			Target.Flying = false;
			Dungeon.Level.Press(Target.pos, Target);
			base.Detach();
		}

		public override int Icon()
		{
			return BuffIndicator.LEVITATION;
		}

		public override string ToString()
		{
			return "Levitating";
		}
	}
}