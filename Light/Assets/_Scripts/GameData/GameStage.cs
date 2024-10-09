using System;

/// <summary>
/// 游戏关卡
/// </summary>
public class GameStage
{
    public GameTime GameTime { get; private set; }
    public PlayableUnit Player { get; private set; }
    public GameStage(PlayableUnit player, int gameTime)
    {
        GameTime = new GameTime(gameTime);
        Player = player;
    }
}

/// <summary>
/// 游戏时间
/// </summary>
public class GameTime
{
    public int TotalSecs { get; private set; }
    public GameTime(int totalSecs)
    {
        TotalSecs = totalSecs;
    }
}