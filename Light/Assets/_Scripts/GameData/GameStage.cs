using System;

/// <summary>
/// 游戏关卡
/// </summary>
public class GameStage : ModelBase
{
    public StageTime Time { get; private set; }
    public PlayableUnit Player { get; private set; }
    public GameStage(PlayableUnit player, int gameTime)
    {
        Time = new StageTime(gameTime);
        Player = player;
    }
}

/// <summary>
/// 游戏时间
/// </summary>
public class StageTime : ModelBase
{
    public int TotalSecs { get; private set; }
    public StageTime(int totalSecs)
    {
        TotalSecs = totalSecs;
    }
}