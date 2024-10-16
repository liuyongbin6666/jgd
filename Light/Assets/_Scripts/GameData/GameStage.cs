using System;
using GMVC.Core;
using UnityEngine;

/// <summary>
/// 游戏关卡
/// </summary>
public class GameStage : ModelBase
{
    public enum PlayMode
    {
        Story,Explore
    }
    public StageIndex StageIndex { get; private set; }
    public StageTime Time { get; private set; }
    public PlayableUnit Player { get; private set; }
    public StoryManager StoryManager { get; private set; }
    public PlayMode Mode { get; private set; }
    public GameStage(PlayableUnit player, StageIndex stageIndex, StageTime stageTime)
    {
        Player = player;
        StageIndex = stageIndex;
        Time = stageTime;
        StoryManager = new StoryManager();
    }

    //开始关卡时设置
    public void Stage_Start()
    {
        Time.StartTimer();
        StoryManager.SetStory();
        SetMode(PlayMode.Explore);
        Game.SendEvent(GameEvent.Story_Npc_Update, 0);
    }
    public void SetMode(PlayMode mode)
    {
        Mode = mode;
        Game.SendEvent(GameEvent.Game_PlayMode_Update, mode);
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
    int _totalSecs;
    public StageTime(StageTimeComponent stageTimeComponent,int remainSeconds)
    {
        StageTimeComponent = stageTimeComponent;
        stageTimeComponent.OnPulseTrigger.AddListener(OnPulse);
        _totalSecs = remainSeconds;
        RemainSeconds = remainSeconds;
    }
    public void StartTimer() => StageTimeComponent.StartCountdown();
    public void Reset()
    {
        StageTimeComponent.StopCountdown();
        RemainSeconds = _totalSecs;
    }
    void OnPulse(int arg0)
    {
        RemainSeconds--;
        SendEvent(GameEvent.Stage_StageTime_Update);
    }
}