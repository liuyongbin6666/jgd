using System;
using System.Collections;
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
    public bool IsRunning { get; private set; }
    Coroutine countdownCoroutine;

    public void StartCountdown() => StartCountdown(false);
    public void StartCountdown(bool reset)
    {
        if (PulseTimes <= 0) throw new NotImplementedException("跳动次数至少要一次!");
        if (Duration <= 0) throw new NotImplementedException("持续市场不可以负数!");
        IsRunning = countdownCoroutine != null;
        if (reset || !IsRunning)
        {
            StopCountdown();
            countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }
    }

    public void StopCountdown()
    {
        if (!IsRunning) return;
        if (countdownCoroutine != null) return;
        StopCoroutine(countdownCoroutine);
        countdownCoroutine = null;
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