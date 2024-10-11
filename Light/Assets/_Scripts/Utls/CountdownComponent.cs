using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public abstract class CountdownComponent : MonoBehaviour
{
    /// <summary>
    /// 跳动次数
    /// </summary>
    protected abstract int PulseTimes { get; }
    /// <summary>
    /// 总持续时间
    /// </summary>
    protected abstract float Duration { get; }

    public readonly UnityEvent<int> OnPulseTrigger = new();
    public readonly UnityEvent OnCountdownComplete = new();

    private Coroutine countdownCoroutine;

    public void StartCountdown()
    {
        if (PulseTimes <= 0) throw new NotImplementedException("跳动次数至少要一次!");
        if (Duration <= 0) throw new NotImplementedException("持续市场不可以负数!");
        if (countdownCoroutine!=null) return;
        StopCountdown();
        countdownCoroutine = StartCoroutine(CountdownCoroutine());
    }

    public void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }

    IEnumerator CountdownCoroutine()
    {
        var times = PulseTimes;
        var interval = Duration / PulseTimes;

        while (times > 0)
        {
            yield return new WaitForSeconds(interval);
            times--;

            OnPulseTrigger?.Invoke(times);
            OnPulse(times); // 调用可被子类重写的方法
        }
        countdownCoroutine = null;
        OnCountdownComplete?.Invoke(); // 倒计时完成后的回调
    }

    // 可被子类重写的回调方法
    protected virtual void OnPulse(int remainingTimes){}

}