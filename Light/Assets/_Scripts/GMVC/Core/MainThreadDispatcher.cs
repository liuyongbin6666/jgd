using System;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Events;

namespace GMVC.Core
{
    public interface IMainThreadDispatcher
    {
        void Enqueue(Action action);
    }
    public class MainThreadDispatcher : MonoBehaviour,IMainThreadDispatcher
    {
        static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();
        static MainThreadDispatcher _instance;
        public bool UseCustomExceptionHandler;
        public event UnityAction<Exception> CustomExceptionHandler;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                //DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Enqueue(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
#if UNITY_EDITOR
            action();//再编辑器模式下直接执行
#else
            _executionQueue.Enqueue(action);
#endif
        }

        void Update()
        {
            while (_executionQueue.TryDequeue(out var action))
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    // 日志记录或其他异常处理
                    //Debug.LogError($"Exception occurred during MainThreadDispatcher action: {ex}");
                    if (UseCustomExceptionHandler) CustomExceptionHandler?.Invoke(ex);
                    else Debug.LogException(ex);
                }
            }
        }
    }
}