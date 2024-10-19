using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 情节机制的基类，用于处理情节的触发和回收。
/// </summary>
[RequireComponent(typeof(PlotComponentBase))]public abstract class PlotSensorBase: SensorListenerBase
{
    enum Control
    {
        [InspectorName("开始")]Begin,
        [InspectorName("完成")]Finalize
    }

    [SerializeField,LabelText("传感控制")] Control control;
    [SerializeField,LabelText("情节控件")] protected PlotComponentBase plotComponent;

    protected override void OnTrigger()
    {
        switch (control)
        {
            case Control.Begin:
                plotComponent.Begin();
                break;
            case Control.Finalize: 
                plotComponent.Finalization();
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}