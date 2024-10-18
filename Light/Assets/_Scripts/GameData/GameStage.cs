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
    public StageStory Story { get; private set; }
    public PlayableUnit Player { get; private set; }
    public PlayMode Mode { get; private set; }
    public GameStage(PlayableUnit player, StageIndex stageIndex, StageStory stageStory)
    {
        Player = player;
        StageIndex = stageIndex;
        Story = stageStory;
    }

    //开始关卡时设置
    public void Stage_Start()
    {
        Story.StartTimer();
        Story.SetStory(new[] { 1, 2, 3, 4, 5 });
        SetMode(PlayMode.Explore);
        //Game.SendEvent(GameEvent.Story_Npc_Update, 0);
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
/// 关卡故事
/// </summary>
public class StageStory : ModelBase
{
    public StageTimeComponent StageTimeComponent;
    public PlotManager PlotManager => Game.PlotManager;
    public int RemainSeconds { get; private set; }
    //故事Id
    int[] StoryId { get; set; }
    int _totalSecs;
    public StageStory(StageTimeComponent stageTimeComponent, int remainSeconds)
    {
        StageTimeComponent = stageTimeComponent;
        stageTimeComponent.OnPulseTrigger.AddListener(OnPulse);
        _totalSecs = remainSeconds;
        RemainSeconds = remainSeconds;
        PlotManager.OnLineEvent.AddListener(OnLine);
    }
    void OnLine(string line) => SendEvent(GameEvent.Story_Line_Send, line);
    public void SetStory(int[] ids)
    {
        StoryId = ids;
    }
    public int GetStoryId()
    {
        return StoryId[Game.World.Stage.StageIndex.Index];
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