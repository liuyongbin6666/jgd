using System.Collections;
using System.Collections.Generic;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    /// <summary>
    /// 情节控件 - IsFinalize 控制是否触发"完成"
    /// </summary>
    public class PlotComponent : PlotComponentBase
    {
        enum TextMode
        {
            [InspectorName("情节开启")]Begin,
            [InspectorName("情节完成")]Finalize,
            [InspectorName("不自动播放")] None
        }
        [SerializeField,LabelText("开始触发")]readonly UnityEvent onTrigger;
        [SerializeField,LabelText("台词间隔秒")]float lineInterval = 1f;
        [LabelText("情节(自动)完成")]public bool IsFinalize;
        [SerializeField,LabelText("文本播放在")] TextMode mode;
        [SerializeField,LabelText("完结奖励")]List<GameObject> Rewards;
        protected override void OnBegin()
        {
            StartCoroutine(BeginRoutine());
            return;

            IEnumerator BeginRoutine()
            {
                onTrigger?.Invoke();
                yield return null;
                if (mode == TextMode.Begin) SendLines();
                yield return new WaitUntil(() => IsFinalize);
                Finalization();
            }
        }
        protected override void OnFinalization()
        {
            if(mode == TextMode.Finalize)SendLines();
            foreach (var reward in Rewards) 
                reward.Display(true);
        }
    }
}