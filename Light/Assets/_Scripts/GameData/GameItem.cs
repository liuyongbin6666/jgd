using UnityEngine;

namespace GameData
{
    public enum GameItemType
    {
        [InspectorName("萤火虫")] Firefly,
        Boss,
        Bullet,
    }

    public interface IGameItem
    {
        GameItemType Type { get; }
        void Invoke(PlayableUnit player);
    }
}