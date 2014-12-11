using System;
using System.Collections.Generic;
using sharpdungeon.actors.blobs;
using sharpdungeon.ui;

namespace sharpdungeon.actors.buffs
{
	public class GasesImmunity : FlavourBuff
	{
		public const float Duration = 5f;

		public override int Icon()
		{
			return BuffIndicator.IMMUNITY;
		}

		public override string ToString()
		{
			return "Immune to gases";
		}

        public static readonly HashSet<Type> Immunities = new HashSet<Type>();
		static GasesImmunity()
		{
			Immunities.Add(typeof(Paralysis));
			Immunities.Add(typeof(ToxicGas));
		}
	}
}