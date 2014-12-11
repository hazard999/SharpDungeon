namespace sharpdungeon.actors.mobs
{
    public interface IAiState
    {
        bool Act(bool enemyInFov, bool justAlerted);
        string Status();
    }
}