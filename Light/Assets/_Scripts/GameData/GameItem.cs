using UnityEngine;

public enum GameItemType
{
    [InspectorName("萤火虫")] Firefly,
    [InspectorName("墓碑")] Grave,
    Boss,
}

public interface IGameItem
{
    GameItemType Type { get; }
    void Invoke(PlayableUnit player);
}