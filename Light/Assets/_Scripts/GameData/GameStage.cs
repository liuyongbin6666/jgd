using System;
using GMVC.Core;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 游戏关卡
/// </summary>
public class GameStage : ModelBase
{
    public enum PlayModes
    {
        Story,Explore
    }
    public StageIndex StageIndex { get; private set; }
    public StageStory Story { get; private set; }
    public PlayableUnit Player { get; private set; }
    public PlayModes Mode { get; private set; } = PlayModes.Explore;//暂时默认探索模式
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
        //SetMode(PlayModes.Explore);
        //Game.SendEvent(GameEvent.Story_Npc_Update, 0);
    }
    //public void SetMode(PlayModes mode)
    //{
    //    Mode = mode;
    //    Game.SendEvent(GameEvent.Game_PlayMode_Update, mode);
    //}
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
    public enum Lines
    {
        [InspectorName("故事")]Story, 
        [InspectorName("对话")]Dialog
    }
    public StageTimeComponent StageTimeComponent;
    public PlotManager PlotManager => Game.PlotManager;
    public int RemainSeconds { get; private set; }
    public string[] StoryLines { get; private set; }
    public string[] DialogLines { get; private set; }
    //故事Id
    int[] StoryId { get; set; }
    int _totalSecs;
    PlotComponentBase currentPlot;
    public StageStory(StageTimeComponent stageTimeComponent, int remainSeconds)
    {
        StageTimeComponent = stageTimeComponent;
        stageTimeComponent.OnPulseTrigger.AddListener(OnPulse);
        _totalSecs = remainSeconds;
        RemainSeconds = remainSeconds;
        PlotManager.OnLinesEvent.AddListener(OnLine);
        PlotManager.OnPlotBegin.AddListener(SetCurrentPlot);
    }

    void SetCurrentPlot(PlotComponentBase? plot)
    {
        var lastPlot = currentPlot;
        currentPlot = plot;
        if (currentPlot)
            SendEvent(GameEvent.Story_Plot_Begin);
        else
            SendEvent(GameEvent.Story_End, lastPlot.story.Name);
    }

    void OnLine(Lines lineType, string[] lines)
    {
        switch (lineType)
        {
            case Lines.Story:
                StoryLines = lines;
                SendEvent(GameEvent.Story_Lines_Send);
                break;
            case Lines.Dialog:
                DialogLines = lines;
                SendEvent(GameEvent.Story_Dialog_Send);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(lineType), lineType, null);
        }
    }

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

    public void Plot_Next() => PlotManager.TriggerNext(currentPlot);
}