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
    public GameStage(PlayableUnit player, StageIndex stageIndex, StageTime stageTime)
    {
        Player = player;
        StageIndex = stageIndex;
        Time = stageTime;
        Time.StartCountdown();
        StoryManager = new StoryManager();
    }

    //开始关卡时设置
    public void SetStage()
    {
        Time.SetTime();
        StoryManager.SetStory();
        Game.SendEvent(GameEvent.Story_Npc_Update, 0);
    }

    //通过关卡
    public void AddStageIndex()
    {
        StageIndex.Add();
    }

}

/// <summary>
/// 游戏时间
/// </summary>
public class StageTime : ModelBase
{
    public StageTimeComponent StageTimeComponent;
    public int RemainSeconds { get; private set; }

    public void StartCountdown()
    {
        StageTimeComponent.StartCountdown();
    }

    public StageTime(StageTimeComponent stageTimeComponent)
    {
        StageTimeComponent = stageTimeComponent;
        stageTimeComponent.OnPulseTrigger.AddListener(OnPulse);
    }

    public void SetTime()
    {
        RemainSeconds = Game.Config.StageSeconds[Game.World.Stage.StageIndex.Index];
    }

    private void OnPulse(int arg0)
    {
        RemainSeconds--;
        SendEvent(GameEvent.Stage_StageTime_Update);
    }
}