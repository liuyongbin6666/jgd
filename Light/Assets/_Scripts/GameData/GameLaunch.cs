using GMVC.Core;
using UnityEngine;

public class GameLaunch : MonoBehaviour
{
    public UiManager UiManager;
    public GameConfig GameConfig;
    void Start()
    {
        Game.Run(GameStart, GameConfig);
    }

    // 游戏开始
    void GameStart()
    {
        UiManager.Init();
        var gameController = Game.GetController<GameController>();
        gameController.Game_Start();
    }
}