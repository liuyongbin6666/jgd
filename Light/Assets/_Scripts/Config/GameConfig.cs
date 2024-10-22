using System;
using Components;

namespace Config
{
    [Serializable]public class GameConfig
    {
        public PlayerControlComponent PlayerPrefab;
        public StageTimeComponent StageTimeComponent;
        public StoryItemDataSO StoryItemDataSo;
        public StorySo[] Stories;
    }
}
