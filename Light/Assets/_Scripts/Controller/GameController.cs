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
        World.InitStage(new PlayableUnit(Config.PlayerPrefab, 1, 1),
                        new StageIndex(),
                        new StageStory(Config.StageTimeComponent,
                                       () => SwitchPlayMode(GameStage.PlayModes.Story),
                                       Config.StageSeconds[0]));
        World.StartStage();
        Debug.Log("游戏执行中！");
    }
    public void SwitchPlayMode(GameStage.PlayModes mode)
    {
        World.Stage.SetMode(mode);
    }
    public void Game_End()
    {
        World.End();
        Debug.Log("游戏结束！");
    }
}