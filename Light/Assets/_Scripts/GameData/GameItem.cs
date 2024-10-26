using UnityEngine;

namespace GameData
{
    public enum GameItemType
    {
        [InspectorName("萤火虫")] Firefly,
        [InspectorName("魔法")] Spell,
        [InspectorName("魂魄")] Soul,
        [InspectorName("血药水")] Portion_HP,
    }

    public interface IGameItem
    {
        GameItemType Type { get; }
        void Invoke(PlayableUnit player);
    }
}