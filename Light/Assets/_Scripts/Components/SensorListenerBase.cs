using UnityEngine;

/// <summary>
/// 传感器基类，基于<seealso cref="SensorManager"/>实现触发事件<br/>
/// 主要实现触发条件监听并触发事件
/// </summary>
public abstract class SensorListenerBase : MonoInitializer
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

    /// <summary>
    /// 是否可触发，用于<seealso cref="CheckCondition"/>的前置条件判断。主要是为了情节的节点顺序服务<br/>
    /// 默认是根据gameObject的激活状态判断
    /// </summary>
    protected virtual bool IsTriggerable => gameObject.activeSelf;
    public override void Initialization()
    {
        SensorManager.RegisterTrigger(this, OnCallback);
        OnInit();
    }
    protected abstract void OnInit();

    // 触发条件
    protected abstract bool CheckCondition();

    bool OnCallback()
    {
        if (!IsTriggerable) return false; // 不可触发，跳过
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