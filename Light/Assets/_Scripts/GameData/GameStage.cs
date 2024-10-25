using System;
using Components;
using GMVC.Core;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// 游戏关卡
    /// </summary>
    public class GameStage : ModelBase
    {
        public enum PlayModes
        {
            Story,Explore
        }
        public StageStory Story { get; private set; }
        public PlayableUnit Player { get; private set; }
        public PlayModes Mode { get; private set; } = PlayModes.Explore;//暂时默认探索模式
        public GameStage(PlayableUnit player, StageStory stageStory)
        {
            Player = player;
            Story = stageStory;
        }

        //开始关卡时设置
        public void Stage_Start()
        {
            Story.StartTimer();
            Player.Enable(true);
            Game.FireflySpawner.StartService(Player.PlayerControl);
            Game.EnemySpawner.StartService(Player.PlayerControl);
            //SetMode(PlayModes.Explore);
            //Game.SendEvent(GameEvent.Story_Npc_Update, 0);
        }
        public void Stage_End()
        {
            Story.Reset();
            Player.Enable(false);
            Game.FireflySpawner.StopService();
            Game.EnemySpawner.StopService();
        }
        //public void SetMode(PlayModes mode)
        //{
        //    Mode = mode;
        //    Game.SendEvent(GameEvent.Game_PlayMode_Update, mode);
        //}
        //通过关卡
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
        public void StartTimer() => StageTimeComponent.StartCountdown();
        public void Reset()
        {
            StageTimeComponent.StopCountdown();
            RemainSeconds = _totalSecs;
        }
        void OnPulse(int arg0)
        {
            RemainSeconds--;
            SendEvent(RemainSeconds > 0 ? GameEvent.Stage_StageTime_Update : GameEvent.Stage_StageTime_Over);
        }

        public void Plot_Next() => PlotManager.TriggerNext(currentPlot);
    }
}