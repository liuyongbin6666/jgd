using System;
using System.Collections.Generic;
using System.Linq;
using GMVC.Data;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class PlotSoBase : AutoNameSoBase
{
    [OnValueChanged(nameof(RegStory))]public StorySo story;

    [LabelText("故事结束")]public bool isStoryFinalize;
    protected override string Prefix { get; } = "情节-";
    [LabelText("自动开启")]public bool autoBegin;
    [TextArea,LabelText("台词")] public string[] lines;
    public abstract string[] NextPlots();
    void RegStory() => story?.RegPlot(this);
    public void RemoveStory() => story = null;
    // 你可以在这里添加更多的配置项，例如触发条件的参数
    public IEnumerable<string> GetPlotNames() => story?.GetPlotNames().Where(n => n != Name) ?? Array.Empty<string>();
}

public class BattlePlot
{

}