using System;
using GMVC.Core;
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
    public GameStage(PlayableUnit player)
    {
        Player = player;
        StageIndex = new StageIndex();
        Time = new StageTime();
        StoryManager = new StoryManager();
        StoryManager.SetStory();
    }

    //通过关卡
    public void AddStageIndex()
    {
        StageIndex.Add();
    }

    //进入新关卡
    public void SetGameStage()
    {
        Time.SetStageTime(Game.Config.StageSeconds[StageIndex.Index]);
    }
}

/// <summary>
/// 游戏时间
/// </summary>
public class StageTime : ModelBase
{
    public int TotalSecs { get; private set; }

    public void SetStageTime(int sec)
    {
        TotalSecs = sec;
    }
}