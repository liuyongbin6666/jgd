using System;
using Components;
using Sirenix.OdinInspector;

namespace Config
{
    [Serializable]public class GameConfig
    {
        [LabelText("法术配置")]public SpellSo SpellSo;
        [LabelText("玩家预设物")]public PlayerControlComponent PlayerPrefab;
        [LabelText("玩家配置")]public PlayerCfgSo PlayerCfgSo;
        [LabelText("故事")]public StorySo[] Stories;
        [LabelText("关卡时间")]public StageTimeComponent StageTimeComponent;
        public StoryItemDataSO StoryItemDataSo;
    }
}
