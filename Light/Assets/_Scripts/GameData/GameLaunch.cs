using System;
using System.Collections;
using System.Linq;
using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameLaunch : MonoBehaviour
{
    public UiManager UiManager;
    public GameConfigure Configure;
    public AudioComponent AudioComponent;
    public PlotManager PlotManager;
    public SensorManager SensorManager;
    public EnvironmentComponent EnvironmentComponent;
    void Start()
    {
        Game.Run(GameStart, 
                 AudioComponent, 
                 Configure.GameConfig, 
                 SensorManager, 
                 PlotManager,
                 EnvironmentComponent);
    }

    // 游戏开始
    void GameStart()
    {
        UiManager.Init();
        var initializers = Resources.FindObjectsOfTypeAll<GameStartInitializer>();
        foreach (var initializer in initializers) initializer.Initialization();
    }
}

/// <summary>
/// 实行自动初始化的MonoBehaviour基类<br/>
/// 注意一般上是使用在必须呀<see cref="Game"/>Game控制的组件上，<br/>
/// 如果需要游戏中可控的组件请使用<see cref="Game.Run"/><br/>
/// 注册在<see cref="Game"/>静态类中
/// </summary>
public abstract class GameStartInitializer : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField,LabelText("自动初始化-测试用")] protected bool autoInitInEditorOnly;
    void Start()
    {
        if (autoInitInEditorOnly) Initialization();
    }
#endif
    public abstract void Initialization();
}