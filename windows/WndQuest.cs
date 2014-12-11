using sharpdungeon.actors;
using sharpdungeon.utils;

namespace sharpdungeon.windows
{
    public class WndQuest : WndTitledMessage
    {
        public WndQuest(Character questgiver, string text) :
            base(questgiver.Sprite, Utils.Capitalize(questgiver.Name), text)
        {
        }
    }
}