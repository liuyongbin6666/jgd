using System;
using System.Collections;
using GMVC.Utls;
using UnityEngine;
using UnityEngine.Events;

namespace GMVC.Core
{
    public static class Game
    {
        static MonoService _monoService;
        static bool IsRunning { get; set; }
        static ControllerServiceContainer ServiceContainer { get; set; }
        public static GameWorld World { get; } = new ();
        public static PlotManager PlotManager { get; private set; }
        public static SensorManager SensorManager { get; private set; }

        public static T GetController<T>() where T : class, IController => ServiceContainer.Get<T>();
        public static UiBuilder UiBuilder { get; private set; }
        public static MessagingManager MessagingManager { get; } = new MessagingManager();
        public static IMainThreadDispatcher MainThread { get; private set; }
        public static Res Res { get; private set; }
        public static AudioComponent AudioComponent { get; private set; }
        public static GameConfig Config { get; private set; }
        static EnvironmentComponent Environment { get; set; }
        public static MonoService MonoService
        {
            get
            {
                if (_monoService == null)
                    _monoService = new GameObject("MonoService").AddComponent<MonoService>();
                return _monoService;
            }
        }
        static Res _res;

        public static void Run(Action onGameStartAction, AudioComponent audioComponent, 
            GameConfig config,
            GameRender renderMode,
            SensorManager sensorManager,
            PlotManager plotManager,
            EnvironmentComponent environmentComponent,
            float startAfter = 0.5f)
        {
            if (IsRunning) throw new NotImplementedException("Game is running!");
            IsRunning = true;
            RenderMode = renderMode;
            SensorManager = sensorManager;
            AudioComponent = audioComponent;
            AudioComponent.Init();
            PlotManager = plotManager;
            Config = config;
            Environment = environmentComponent;
            Environment.Init();
            MainThread = MonoService.gameObject.AddComponent<MainThreadDispatcher>();
            Res = MonoService.gameObject.AddComponent<Res>();
            ControllerReg();
            RegEvents();
            MonoService.StartCoroutine(StartAfterSec(startAfter));
            return;

            void ControllerReg()
            {
                ServiceContainer = new ControllerServiceContainer();
                ServiceContainer.Reg(new GameController());
                ServiceContainer.Reg(new PlayableController());
            }

            void RegEvents() { }

            IEnumerator StartAfterSec(float delay)
            {
                yield return new WaitForSeconds(delay);
                onGameStartAction?.Invoke();
            }
        }

        public static GameRender RenderMode { get; private set; }
        public static void SendEvent(string eventName, DataBag bag) => MessagingManager.Send(eventName, bag);
        public static void SendEvent(string eventName, params object[] args)
        {
            args ??= Array.Empty<object>();
            MessagingManager.Send(eventName, args);
        }
        public static void RegEvent(string eventName, Action<DataBag> callbackAction) =>
            MessagingManager.RegEvent(eventName, callbackAction);
        public static void PlayBGM(AudioClip clip) => AudioComponent.Play(AudioManager.Types.BGM,clip);
        public static void PlaySFX(AudioClip clip) => AudioComponent.Play(AudioManager.Types.SFX, clip);

        /// <summary>
        /// 开始协程服务
        /// </summary>
        /// <param name="coroutineFunc"></param>
        /// <returns></returns>
        public static Coroutine StartCoService(Func<GameWorld, IEnumerator> coroutineFunc)
        {
            return _monoService.StartCoroutine(coroutineFunc.Invoke(World));
        }

        /// <summary>
        /// 停止协程服务
        /// </summary>
        /// <param name="coroutine"></param>
        public static void StopCoService(Coroutine coroutine)
        {
            if (coroutine == null) return;
            _monoService.StopCoroutine(coroutine);
        }
    }
}
