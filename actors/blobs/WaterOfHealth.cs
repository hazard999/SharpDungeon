using pdsharp.noosa.audio;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects;
using sharpdungeon.effects.particles;
using sharpdungeon.items;
using sharpdungeon.items.potions;
using sharpdungeon.utils;

namespace sharpdungeon.actors.blobs
{
    public class WaterOfHealth : WellWater
    {
        private const string TxtProcced = "As you take a sip, you feel your wounds heal completely.";

        protected internal override bool AffectHero(Hero hero)
        {
            Sample.Instance.Play(Assets.SND_DRINK);

            PotionOfHealing.Heal(hero);
            hero.Belongings.UncurseEquipped();
            hero.Buff<Hunger>().Satisfy(Hunger.Starving);

            CellEmitter.Get(Pos).Start(ShaftParticle.Factory, 0.2f, 3);

            Dungeon.Hero.Interrupt();

            GLog.Positive(TxtProcced);

            Journal.Remove(Journal.Feature.WELL_OF_HEALTH);

            return true;
        }

        protected internal override Item AffectItem(Item item)
        {
            if (!(item is DewVial) || ((DewVial) item).IsFull) 
               return null;

            ((DewVial)item).Fill();;
            return item;
        }

        public override void Use(BlobEmitter emitter)
        {
            base.Use(emitter);
            emitter.Start(Speck.Factory(Speck.HEALING), 0.5f, 0);
        }

        public override string TileDesc()
        {
            return "Power of health radiates from the water of this well. " + "Take a sip from it to heal your wounds and satisfy hunger.";
        }
    }
}