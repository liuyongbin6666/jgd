using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// 传感器管理器，用于管理传感器的触发事件<br/>
/// 主要用于处理传感器的触发事件，可以自由支持传感策略，并支持检测不同检测机制来减少性能开销。<br/>
/// 主要控件：<seealso cref="SensorListenerBase"/>，<seealso cref="SensorSoBase"/>
/// </summary>
public class SensorManager : MonoBehaviour
{
    readonly Dictionary<SensorListenerBase,Func<bool>> _frameMap = new();
    readonly Dictionary<SensorListenerBase,Coroutine> _coMap = new();

    public void RegisterTrigger(SensorListenerBase sensor, Func<bool> callbackFunc)
    {
        var update = sensor.UpdateStrategy;
        switch (update.detection)
        {
            case UpdateStrategy.Detection.EveryFrame:
            case UpdateStrategy.Detection.FrameInterval:
                _frameMap.Add(sensor, callbackFunc);
                break;
            case UpdateStrategy.Detection.TimeInterval:
                _coMap.Add(
                    sensor, StartCoroutine(
                        update.OnTimeIntervalRoutine(callbackFunc, () => RemoveTrigger(sensor))));
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    public void RemoveTrigger(SensorListenerBase sensor)
    {
        if (_frameMap.ContainsKey(sensor))
            _frameMap.Remove(sensor);
        else if (_coMap.ContainsKey(sensor))
        {
            var co = _coMap[sensor];
            if (co != null) StopCoroutine(co);
            _coMap.Remove(sensor);
        }
    }

    void LateUpdate()
    {
        foreach (var trigger in _frameMap.Keys.ToArray())
        {
            var func = _frameMap[trigger];
            var update = trigger.UpdateStrategy;
            if (!update.UpdateFrameCheck()) continue; // 检查更新策略
            var isFinalized = func.Invoke();
            // 根据触发策略决定是否移除
            if (isFinalized) RemoveTrigger(trigger);
        }
    }
}