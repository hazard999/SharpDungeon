using System.Collections.Generic;
using pdsharp.noosa.particles;
using sharpdungeon.actors.buffs;
using sharpdungeon.actors.hero;
using sharpdungeon.effects.particles;
using sharpdungeon.sprites;

namespace sharpdungeon.items
{
    public class Torch : Item
    {
        public const string AcLight = "LIGHT";

        public const float TimeToLight = 1;

        public Torch()
        {
            name = "torch";
            image = ItemSpriteSheet.TORCH;

            Stackable = true;

            DefaultAction = AcLight;
        }

        public override List<string> Actions(Hero hero)
        {
            var actions = base.Actions(hero);
            actions.Add(AcLight);
            return actions;
        }

        public override void Execute(Hero hero, string action)
        {
            if (action == AcLight)
            {
                hero.Spend(TimeToLight);
                hero.Busy();

                hero.Sprite.DoOperate(hero.pos);

                Detach(hero.Belongings.Backpack);
                Buff.Affect<Light>(hero, Light.Duration);

                var emitter = hero.Sprite.CenterEmitter();
                emitter.Start(FlameParticle.Factory, 0.2f, 3);
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

        public override int Price()
        {
            return 10 * Quantity();
        }

        public override string Info()
        {
            return "It's an indispensable item in The Demon Halls, which are notorious for their poor ambient lighting.";
        }
    }
}