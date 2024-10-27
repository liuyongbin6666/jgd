using GameData;
using UnityEngine.Events;

namespace Components
{
    public class StoryItemComponent : GameItemBase
    {
        public override GameItemType Type => GameItemType.StoryItem;
        public UnityEvent OnInteractEvent;
        public override void Invoke(PlayableUnit player) => OnInteractEvent.Invoke();
    }
}