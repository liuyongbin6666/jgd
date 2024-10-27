using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Components
{
    /// <summary>
    /// 灯笼控件
    /// </summary>
    public class LanternComponent : MonoBehaviour
    {
        [LabelText("虫灯维持秒数")] public float _lastingPerFirefly = 3f;
        public LanternVisionLevelComponent LanternVisionLevel;
        Coroutine lanternCoroutine;
        public UnityEvent OnCountdownComplete { get; } = new();
        public void StartLantern()
        {
            lanternCoroutine = StartCoroutine(StartCountdown());
        }

        IEnumerator StartCountdown()
        {
            while (true)
            {
                yield return new WaitForSeconds(_lastingPerFirefly);
                OnCountdownComplete.Invoke();
            }
        }
        public float SetVisionLevel(int level)
        {
            LanternVisionLevel.LoadLightLevel(level,out var isMaxLevel,out var moveRatio);
            if (isMaxLevel) Restart();
            return moveRatio;
        }
        
        public void Restart()
        {
            StopLantern();
            lanternCoroutine = StartCoroutine(StartCountdown());
        }

        public void StopLantern()
        {
            if (lanternCoroutine != null)
                StopCoroutine(lanternCoroutine);
        }
    }
}