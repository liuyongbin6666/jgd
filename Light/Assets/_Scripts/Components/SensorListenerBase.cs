using UnityEngine;

/// <summary>
/// 传感器基类，基于<seealso cref="SensorManager"/>实现触发事件<br/>
/// 主要实现触发条件监听并触发事件
/// </summary>
public abstract class SensorListenerBase : GameStartInitializer
{
    protected abstract SensorSoBase SensorSo { get; }
    protected abstract SensorManager SensorManager { get; }
    public enum RepeatMode
    {
        [InspectorName("一次")]Once,
        [InspectorName("一直")]Always
    }
    public UpdateStrategy UpdateStrategy => SensorSo.updateStrategy;
    RepeatMode Repeat => SensorSo.repeat;
    bool IsInit { get; set; }
    /// <summary>
    /// 在<seealso cref="CheckCondition"/>之前的检查，用于检查是否可以触发
    /// </summary>
    protected abstract bool IsTriggerable { get; }

    protected override void OnGameStart()
    {
        SensorManager.RegisterTrigger(this, OnCondition);
        OnSensorInit();
        IsInit = true;
    }
    protected abstract void OnSensorInit();

    // 触发条件
    protected abstract bool CheckCondition();

    bool OnCondition()
    {
        if (!IsInit) return false;
        if (!gameObject.activeSelf) return false; // 不可触发，跳过
        if (!IsTriggerable) return false; // 不可触发，跳过
        $"{name}检查CheckCondition = {CheckCondition()}".Log(this);
        if (!CheckCondition()) return false; //条件不满足，跳过
        TriggerEvent();
        return Repeat == RepeatMode.Once; //返回是否finalize
    }


    // 如果需要其他检测策略，可以启动协程或定时器
    void OnDestroy()
    {
        if(SensorManager)
            SensorManager.RemoveTrigger(this);
    }

    // 触发事件
    void TriggerEvent()
    {
        if (SensorSo != null)
        {
            OnTrigger();
        }
        else
        {
            Debug.LogWarning("触发器配置或事件未设置");
        }
    }
    /// <summary>
    /// 当事件触发
    /// </summary>
    protected abstract void OnTrigger();
}