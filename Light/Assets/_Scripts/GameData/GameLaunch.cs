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
        var initializers = Resources.FindObjectsOfTypeAll<MonoInitializer>();
        foreach (var initializer in initializers) initializer.Initialization();
    }
}

/// <summary>
/// 实行自动初始化的MonoBehaviour基类
/// </summary>
public abstract class MonoInitializer : MonoBehaviour
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