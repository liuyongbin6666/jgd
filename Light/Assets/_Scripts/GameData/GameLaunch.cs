using Components;
using Config;
using fight_aspect;
using GMVC.Core;
using Sirenix.OdinInspector;
using Ui;
using UnityEngine;

namespace GameData
{
    public class GameLaunch : MonoBehaviour
    {
        public UiManager UiManager;
        public GameConfigure Configure;
        public AudioComponent AudioComponent;
        public PlotManager PlotManager;
        public SensorManager SensorManager;
        public BulletManager BulletManager;
        public EnvironmentComponent EnvironmentComponent;

        void Start()
        {
            Game.Run(GameStart,
                AudioComponent,
                Configure,
                SensorManager,
                PlotManager,
                BulletManager,
                EnvironmentComponent);
        }

        // 游戏开始
        void GameStart()
        {
            UiManager.Init();
            var initializers = Resources.FindObjectsOfTypeAll<GameStartInitializer>();
            foreach (var initializer in initializers) initializer.GameStart();
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
#endif
        bool isGameStart;
        void Start()
        {
#if UNITY_EDITOR
            if (autoInitInEditorOnly) GameStart();
#endif
            OnStart();
        }
        protected virtual void OnStart(){}
        public void GameStart()
        {
            if(isGameStart)return;
            OnGameStart();
            isGameStart = true;
        }

        protected abstract void OnGameStart();
    }
}