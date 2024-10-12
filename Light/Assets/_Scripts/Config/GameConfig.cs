using System;

[Serializable]public class GameConfig
{
    public PlayerControlComponent PlayerPrefab;

    public int[] StageSeconds;

    public StoryOpenDataSO StoryOpenDataSo;
    public StoryFinishDataSO StoryFinishDataSo;
    public StoryItemDataSO StoryItemDataSo;
}
