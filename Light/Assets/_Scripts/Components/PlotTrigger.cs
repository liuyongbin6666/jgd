using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components
{
    /// <summary>
    /// 情节机制的基类，用于处理情节的触发和回收。
    /// </summary>
    [RequireComponent(typeof(PlotComponentBase))]public abstract class PlotSensorBase: SensorListenerBase
    {
        enum Control
        {
            [InspectorName("开始情节")]Begin,
            [InspectorName("完结情节")]Finalize,
        }
        [SerializeField,LabelText("传感控制")] Control control;
        [SerializeField,LabelText("情节控件")] protected PlotComponentBase plotComponent;
        [SerializeField, LabelText("播放文本")] bool sendDialog;
        protected override bool IsTriggerable => plotComponent.IsCurrentState();
        protected override void OnTrigger()
        {
            switch (control)
            {
                case Control.Begin:
                    plotComponent.Begin();
                    if (sendDialog) plotComponent.SendLines();
                    break;
                case Control.Finalize: 
                    if (sendDialog) plotComponent.SendLines();
                    plotComponent.Finalization();
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}