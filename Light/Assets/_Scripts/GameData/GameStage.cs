using System;
using System.Linq;
using Components;
using Config;
using GMVC.Core;
using UnityEngine;
using Utls;

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
        public void Stage_End(bool complete)
        {
            Story.StopTimer();
            Player.Enable(false);
            Game.FireflySpawner.StopService();
            Game.EnemySpawner.StopService();
            SendEvent(GameEvent.Stage_End, complete);
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
        public int Seconds { get; private set; }
        public string[] StoryLines { get; private set; }
        public string[] DialogLines { get; private set; }
        
        public StageStory(StageTimeComponent stageTimeComponent)
        {
            StageTimeComponent = stageTimeComponent;
            stageTimeComponent.OnSecond.AddListener(OnPulse);
            PlotManager.OnLinesEvent.AddListener(OnLine);
            PlotManager.OnPlotBegin.AddListener(OnPlotBegin);
            PlotManager.OnStoryEnd.AddListener(OnStoryEnd);
            //PlotManager.OnPlotBegin.AddListener(SetCurrentPlot);
        }

        void OnStoryEnd(StorySo endStory, int arg1)
        {
            var otherActiveStories = PlotManager.Stories.Where(s => s != endStory && !PlotManager.IsStoryFinalized(s)).ToList();
            $"End: {endStory.Name}, List = {string.Join(',', PlotManager.Stories.Select(s => s.Name))},\n reactive : {string.Join(',', otherActiveStories.Select(s => s.Name))}"
                .Log(PlotManager);
            foreach (var otherStorySo in otherActiveStories)
                PlotManager.SetActiveStory(otherStorySo, true);
            if (otherActiveStories.Count > 0) return;
            Game.World.Stage.Stage_End(true);
        }

        void OnPlotBegin(PlotComponentBase plot)
        {
            if(plot.story.GetFirstPlotName() != plot.plotName)return;
            $"Begin: {plot.story.Name}, disable = {string.Join(',', PlotManager.Stories.Select(s => s.Name))}"
                .Log(PlotManager);
            foreach (var otherStorySo in PlotManager.Stories.Where(s => s != plot.story))
                PlotManager.SetActiveStory(otherStorySo, false);
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
        public void StartTimer() => StageTimeComponent.StartService(true);
        public void StopTimer() => StageTimeComponent.StopService();
        void OnPulse()
        {
            Seconds++;
            SendEvent(Seconds > 0 ? GameEvent.Stage_StageTime_Update : GameEvent.Stage_StageTime_Over);
        }

    }
}