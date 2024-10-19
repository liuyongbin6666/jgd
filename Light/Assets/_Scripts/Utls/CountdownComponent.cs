using System;
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
    bool IsRunning { get; set; }
    int remainTimes;

    public void StartCountdown() => StartCountdown(false);
    public void StartCountdown(bool reset)
    {
        if (PulseTimes <= 0) throw new NotImplementedException("跳动次数至少要一次!");
        if (Duration <= 0) throw new NotImplementedException("持续市场不可以负数!");
        IsRunning = true;
        var interval = Duration / PulseTimes;
        if (reset) StopCountdown();
        if (IsRunning) return;
        InvokeRepeating(nameof(Execute), PulseTimes, interval);
    }
    public void StopCountdown()
    {
        if (!IsRunning) return;
        CancelInvoke(nameof(Execute));
        remainTimes = 0;
        IsRunning = false;
    }

    void Execute()
    {
        remainTimes--;
        OnPulseTrigger?.Invoke(remainTimes);
        OnPulse(remainTimes); // 调用可被子类重写的方法
        if (remainTimes > 0) return;
        // 倒计时完成后的回调
        OnCountdownComplete?.Invoke();
        StopCountdown();
    }

    // 可被子类重写的回调方法
    protected virtual void OnPulse(int remainingTimes){}
}