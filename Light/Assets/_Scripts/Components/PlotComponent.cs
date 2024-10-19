using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 情节控件 - IsFinalize 控制是否触发"完成"
/// </summary>
public class PlotComponent : PlotComponentBase
{
    [SerializeField,LabelText("开始触发")]readonly UnityEvent onTrigger;
    [SerializeField,LabelText("台词间隔秒")]float lineInterval = 1f;
    [LabelText("情节(自动)完成")]public bool IsFinalize;
    public override void Begin()
    {
        $"{name}: 情节执行".Log(this);
        StartCoroutine(BeginRoutine());

        IEnumerator BeginRoutine()
        {
            onTrigger?.Invoke();
            yield return null;
            var (type,lines) = story.GetLines(plotName);
            PlotManager.SendLines(type, lines.ToArray());
            yield return new WaitUntil(() => IsFinalize);
            Finalization();
        }
    }
}