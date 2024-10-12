using System;
using UnityEngine;

/// <summary>
/// 游戏关卡
/// </summary>
public class GameStage : ModelBase
{
    public StageIndex StageIndex { get; private set; }
    public StageTime Time { get; private set; }
    public PlayableUnit Player { get; private set; }
    public StoryManager StoryManager { get; private set; }
    public GameStage(PlayableUnit player, int gameTime)
    {
        StageIndex = new StageIndex();
        Time = new StageTime(gameTime);
        Player = player;
        StoryManager = new StoryManager();
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