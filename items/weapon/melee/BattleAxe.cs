using sharpdungeon.sprites;

namespace sharpdungeon.items.weapon.melee
{
    public class BattleAxe : MeleeWeapon
    {
        public BattleAxe()
            : base(4, 1.2f, 1f)
        {
            name = "battle axe";
            image = ItemSpriteSheet.BATTLE_AXE;
        }

        public override string Desc()
        {
            return "The enormous steel head of this battle axe puts considerable heft behind each stroke.";
        }
    }
}