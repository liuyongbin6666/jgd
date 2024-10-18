using System.Collections.Generic;
using System.Linq;
using GMVC.Utls;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 情节管理器, 注册并处理所有情节事件，根据情节的配置调用场景中的组件
/// </summary>
public class PlotManager : MonoBehaviour
{
    Dictionary<StorySo, List<PlotComponentBase>> data = new();
    public readonly UnityEvent<string> OnLineEvent = new ();
    public void RegComponent(PlotComponentBase plot)
    {
        if(!data.ContainsKey(plot.story)) data.Add(plot.story, new List<PlotComponentBase>());
        data[plot.story].Add(plot);
    }
    public void UnRegComponent(PlotComponentBase plot)
    {
        if (data.ContainsKey(plot.story)) data[plot.story].Remove(plot);
    }
    public void TriggerNext(PlotComponentBase com)
    {
        if (!data.TryGetValue(com.story, out var list)) return;
        var plotName = com.plotName;
        var startPlots = com.story.BeginNext(plotName);
        com.Display(false);
        var plots = list.Join(startPlots, p => p.plotName, n => n, (p, _) => p).ToArray();
        foreach (var plot in plots)
        {
            plot.Display(true);
            plot.Begin();
        }
    }
    public void SendLine(string line) => OnLineEvent?.Invoke(line);
}