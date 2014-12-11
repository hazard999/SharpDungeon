using sharpdungeon.actors.buffs;
using sharpdungeon.sprites;

namespace sharpdungeon.actors.mobs.npcs
{
    public class RatKing : NPC
    {
        public RatKing()
        {
            Name = "rat king";
            SpriteClass = typeof(RatKingSprite);

            State = Sleepeing;
        }

        public override int DefenseSkill(Character localEnemy)
        {
            return 1000;
        }

        public override float Speed()
        {
            return 2f;
        }

        protected internal override Character ChooseEnemy()
        {
            return Dummy;
        }

        public override void Damage(int dmg, object src)
        {
        }

        public override void Add(Buff buff)
        {
        }

        public override bool Reset()
        {
            return true;
        }

        public override void Interact()
        {
            Sprite.TurnTo(pos, Dungeon.Hero.pos);

            if (State == Sleepeing)
            {
                Notice();
                Yell("I'm not sleeping!");
                State = WANDERING;
            }
            else
                Yell("What is it? I have no time for this nonsense. My kingdom won't rule itself!");
        }
        
        public override string Description()
        {
            return "This rat is a little bigger than a regular marsupial rat " + "and it's wearing a tiny crown on its head.";
        }
    }
}