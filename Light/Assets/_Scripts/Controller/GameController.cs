using Components;
using Config;
using GameData;
using GMVC.Conditions;
using GMVC.Core;
using GMVC.Utls;
using UnityEngine;

namespace Controller
{
    public class GameController : ControllerBase
    {
        GameWorld World => Game.World;
        GameConfig Config => Game.Config;

        public void Game_StartStage()
        {
            if (World.Status != GameWorld.GameStates.Start)
            {
                Debug.LogWarning("游戏状态错误！");
                return;
            }
            Config.PlayerPrefab.Display(false);
            var player = InstancePlayer();
            World.Start();
            World.SetGameStage(player, new StageStory(Config.StageTimeComponent, 180));
            World.StartGameStage();
            Debug.Log("游戏执行中！");
        }

        PlayableUnit InstancePlayer()
        {
            var player = DefaultPlayer();
            var playerControl = Object.Instantiate(Config.PlayerPrefab, Config.PlayerPrefab.transform.parent);
            return new PlayableUnit(player, playerControl);
        }
        Player DefaultPlayer()
        {
            return new Player(new ConValue("血量", 100),
                new ConValue("虫灯", 8, 8, 2));
        }

        //public void SwitchPlayMode(GameStage.PlayModes mode)
        //{
        //    World.Stage.SetMode(mode);
        //    World.Stage.Story.Plot_Next();
        //}
        public void Game_End() => World.End();

        public void Game_NextStage()
        {
            World.NextGameStage();
            World.SetGameStage(InstancePlayer(), new StageStory(Config.StageTimeComponent, 180));
            World.StartGameStage();
        }
    }
}