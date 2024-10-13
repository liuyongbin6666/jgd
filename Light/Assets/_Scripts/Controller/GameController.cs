using GMVC.Core;
using UnityEngine;

public class GameController : IController
{
    GameWorld World => Game.World;
    GameConfig Config => Game.Config;

    public void Game_Playing()
    {
        if (World.Status != GameWorld.GameStates.Start)
        {
            Debug.LogWarning("游戏状态错误！");
            return;
        }
        World.Playing();
        Debug.Log("游戏开始！");
    }

    public void Game_StartStage()
    {
        World.Start(new PlayableUnit(Config.PlayerPrefab, 1, 1),new StageTime(Config.StageTimeComponent,60));
        Game_Playing();//暂时自动开始游戏
        Debug.Log("游戏执行中！");
    }

    public void Game_End()
    {
        World.End();
        Debug.Log("游戏结束！");
    }
}