using System;
using System.Collections;
using GMVC.Utls;
using UnityEngine;

namespace GMVC.Core
{
    public static class Game
    {
        static MonoService _monoService;
        static bool IsRunning { get; set; }
        static ControllerServiceContainer ServiceContainer { get; set; }
        public static GameWorld World { get; } = new ();
        public static T GetController<T>() where T : class, IController => ServiceContainer.Get<T>();
        public static UiBuilder UiBuilder { get; private set; }
        public static MessagingManager MessagingManager { get; } = new MessagingManager();
        public static IMainThreadDispatcher MainThread { get; private set; }
        public static Res Res { get; private set; }
        public static AudioComponent AudioComponent { get; private set; }
        public static GameConfig Config { get; private set; }
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

        public static void Run(Action onGameStartAction, AudioComponent audioComponent, GameConfig config,
            float startAfter = 0.5f)
        {
            if (IsRunning) throw new NotImplementedException("Game is running!");
            IsRunning = true;
            AudioComponent = audioComponent;
            AudioComponent.Init();
            Config = config;
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
            }

            void RegEvents() { }

            IEnumerator StartAfterSec(float delay)
            {
                yield return new WaitForSeconds(delay);
                onGameStartAction?.Invoke();
            }
        }

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
    }
}
