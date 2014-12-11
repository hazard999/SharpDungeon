using pdsharp.noosa.audio;
using sharpdungeon.actors.hero;
using sharpdungeon.effects.particles;
using sharpdungeon.utils;

namespace sharpdungeon.items
{
	public abstract class EquipableItem : Item
	{
        private const string TxtUnequipCursed = "You can't Remove cursed {0}!";

		public const string AcEquip = "EQUIP";
		public const string AcUnequip = "UNEQUIP";

		public override void Execute(Hero hero, string action)
		{
		    if (action.Equals(AcEquip))
		        DoEquip(hero);
		    else if (action.Equals(AcUnequip))
		        DoUnequip(hero, true);
		    else
		        base.Execute(hero, action);
		}

		public override void DoDrop(Hero hero)
		{
		    if (!IsEquipped(hero) || DoUnequip(hero, false, false))
		        base.DoDrop(hero);
		}

		public override void Cast(Hero user, int dst)
		{
		    if (IsEquipped(user))
		        if (quantity == 1 && !DoUnequip(user, false, false))
		            return;

		    base.Cast(user, dst);
		}

		protected internal static void EquipCursed(Hero hero)
		{
			hero.Sprite.Emitter().Burst(ShadowParticle.Curse, 6);
			Sample.Instance.Play(Assets.SND_CURSED);
		}

		protected internal virtual float Time2Equip(Hero hero)
		{
			return 1;
		}

		public abstract bool DoEquip(Hero hero);

		public virtual bool DoUnequip(Hero hero, bool collect, bool single)
		{
			if (cursed)
			{
				GLog.Warning(TxtUnequipCursed, Name);
				return false;
			}

		    if (single)
		        hero.SpendAndNext(Time2Equip(hero));
		    else
		        hero.Spend(Time2Equip(hero));

		    if (collect && !Collect(hero.Belongings.Backpack))
		        Dungeon.Level.Drop(this, hero.pos);

		    return true;
		}

		public virtual bool DoUnequip(Hero hero, bool collect)
		{
			return DoUnequip(hero, collect, true);
		}
	}
}