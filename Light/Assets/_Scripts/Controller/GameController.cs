using GMVC.Conditions;
using GMVC.Core;
using UnityEngine;

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
        var player = new Player(new ConValue("血量", 100), new ConValue("魔法", 100));
        World.InitStage(new PlayableUnit(player,Config.PlayerPrefab, 1, 1),
                        new StageIndex(),
                        new StageStory(Config.StageTimeComponent, 180));
        World.StartStage();
        Debug.Log("游戏执行中！");
    }
    public void SwitchPlayMode(GameStage.PlayModes mode)
    {
        World.Stage.SetMode(mode);
        World.Stage.Story.Plot_Next();
    }
    public void Game_End()
    {
        World.End();
        Debug.Log("游戏结束！");
    }
}