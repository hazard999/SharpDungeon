namespace sharpdungeon.actors.buffs
{
    //Special kind of buff, that doesn't perform any kind actions 
    public class FlavourBuff : Buff
    {
        protected override bool Act()
        {
            Detach();
            return true;
        }
    }
}