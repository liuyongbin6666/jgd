using System;
using System.Collections.Generic;
using System.Linq;
using GMVC.Data;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "故事", menuName = "配置/故事")]
public class StorySo : AutoNameSoBase
{

    [OnValueChanged(nameof(ResetPlots))]public List<PlotSoBase> plots;
    void ResetPlots()
    {
        foreach (var plot in plots) plot.story = this;
    }
    [Button("清除列表")]public void ClearList()
    {
        foreach (var plot in plots.ToArray())
        {
            plots.Remove(plot);
            plot.RemoveStory();
        }
    }
    public IEnumerable<string> GetPlotNames() => plots?.Select(p => p.Name) ?? Array.Empty<string>();
    public void RegPlot(PlotSoBase plot)
    {
        if (plots.Contains(plot)) return;
        plots.Add(plot);
    }
    public IEnumerable<string> BeginNext(string plotName)
    {
        var plot = GetPlot(plotName);
        return plot.NextPlots();
    }

    public bool IsAutoBegin(string plotName)
    {
        var plot = GetPlot(plotName);
        
        return plot.autoBegin;
    }

    PlotSoBase GetPlot(string plotName)
    {
        var plot = plots.FirstOrDefault(p => p.Name == plotName);
        if (!plot) throw new NotImplementedException($"{name}:找不到剧情：{plotName}");
        return plot;
    }

    public (StageStory.Lines,string[]) GetLines(string plotName)
    {
        var plot = GetPlot(plotName);
        var lines =plot?.lines ?? Array.Empty<string>();
        return (plot?.lineType ?? 0, lines);
    }
}