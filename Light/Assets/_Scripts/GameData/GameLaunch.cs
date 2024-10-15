using GMVC.Core;
using UnityEngine;

public class GameLaunch : MonoBehaviour
{
    public GameRender RenderMode;
    public UiManager UiManager;
    public GameConfigure Configure;
    public AudioComponent AudioComponent;
    public EnvironmentComponent EnvironmentComponent;
    void Start()
    {
        Game.Run(GameStart, AudioComponent, Configure.GameConfig, RenderMode, EnvironmentComponent);
    }

    // 游戏开始
    void GameStart()
    {
        UiManager.Init();
    }
}