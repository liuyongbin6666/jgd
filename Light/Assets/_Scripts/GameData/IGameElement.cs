namespace GameData
{
    /// <summary>
    /// 游戏元素
    /// </summary>
    public interface IGameElement
    {
        string Type { get; }
        string Name { get; }
    }
}