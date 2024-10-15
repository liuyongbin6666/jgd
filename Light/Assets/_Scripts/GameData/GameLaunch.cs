using GMVC.Core;
using UnityEngine;

public class GameLaunch : MonoBehaviour
{
    public GameRender RenderMode;
    public UiManager UiManager;
    public GameConfigure Configure;
    public AudioComponent AudioComponent;
    void Start()
    {
        Game.Run(GameStart, AudioComponent, Configure.GameConfig, RenderMode);
    }

    // 游戏开始
    void GameStart()
    {
        UiManager.Init();
    }
}