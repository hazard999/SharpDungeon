using System.Collections.Generic;
using pdsharp.noosa.audio;
using sharpdungeon.actors.hero;
using sharpdungeon.effects.particles;
using sharpdungeon.scenes;
using sharpdungeon.sprites;
using sharpdungeon.utils;
using sharpdungeon.windows;
using sharpdungeon.items.armor;

namespace sharpdungeon.items
{
    public class Stylus : Item
    {
        private const string TxtSelectArmor = "Select an armor to inscribe on";
        private const string TxtInscribed = "you inscribed the {0} on your {1}";

        private const float TimeToInscribe = 2;

        private const string AcInscribe = "INSCRIBE";

        public Stylus()
        {
            name = "arcane stylus";
            image = ItemSpriteSheet.STYLUS;

            Stackable = true;
            _itemSelector = new StylusListener(this);
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);

            actions.Add(AcInscribe);

            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action == AcInscribe)
            {
                CurUser = hero;
                GameScene.SelectItem(_itemSelector, WndBag.Mode.ARMOR, TxtSelectArmor);
            }
            else
                base.Execute(hero, action);
        }

        public override bool Upgradable
        {
            get { return false; }
        }

        public override bool Identified
        {
            get { return true; }
        }

        public void Inscribe(Armor armor)
        {
            Detach(CurUser.Belongings.Backpack);

            var oldGlyphClass = armor.glyph != null ? armor.glyph.GetType() : null;

            var glyph = Glyph.Random();

            while (glyph.GetType() == oldGlyphClass)
                glyph = Glyph.Random();

            GLog.Warning(TxtInscribed, glyph.Name(), armor.Name);

            armor.Inscribe(glyph);

            CurUser.Sprite.DoOperate(CurUser.pos);
            CurUser.Sprite.CenterEmitter().Start(PurpleParticle.Burst, 0.05f, 10);
            Sample.Instance.Play(Assets.SND_BURNING);

            CurUser.Spend(TimeToInscribe);
            CurUser.Busy();
        }

        public override int Price()
        {
            return 50 * Quantity();
        }

        public override string Info()
        {
            return "This arcane stylus is made of some dark, very hard stone. Using it you can inscribe " + "a magical glyph on your armor, but you have no power over choosing what glyph it will be, " + "the stylus will decide it for you.";
        }

        private readonly WndBag.Listener _itemSelector;
    }

    internal class StylusListener : WndBag.Listener
    {
        private readonly Stylus _stylus;

        public StylusListener(Stylus stylus)
        {
            _stylus = stylus;
        }

        public void OnSelect(Item item)
        {
            if (item != null)
                _stylus.Inscribe((Armor)item);
        }
    }
}