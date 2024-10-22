using System.Collections.Generic;
using System.Linq;
using Config;
using GameData;
using GMVC.Utls;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    /// <summary>
    /// 情节管理器, 注册并处理所有情节事件，根据情节的配置调用场景中的组件
    /// </summary>
    public class PlotManager : MonoBehaviour
    {
        Dictionary<StorySo, List<PlotComponentBase>> data = new();
        public readonly UnityEvent<StageStory.Lines, string[]> OnLinesEvent = new();
        public readonly UnityEvent<PlotComponentBase> OnPlotBegin = new();
        Dictionary<StorySo, string> currentMap = new();

        public void Init(StorySo[] stories)
        {
            foreach (var story in stories)
            {
                data.Add(story,new List<PlotComponentBase>());
                currentMap.Add(story,story.GetFirstPlot());
            }
        }

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
            var storyEnd = com.story.IsStoryEnd(plotName);
            com.Display(false);
            if (storyEnd)
            {
                SetCurrentPlot(null);
                return;
            }
            var nextPlots = com.story.NextPlots(plotName);
            var plots = list.Join(nextPlots, p => p.plotName, n => n, (p, _) => p).ToArray();
            foreach (var plot in plots)
            {
                plot.Display(true);
                plot.Begin();
            }
        }
        public void SendLines(StageStory.Lines type, string[] lines) => OnLinesEvent?.Invoke(type, lines);

        public void SetCurrentPlot(PlotComponentBase plot)
        {
            if (plot) currentMap[plot.story] = plot.plotName;
            OnPlotBegin.Invoke(plot);
        }

        public bool IsCurrentPlot(PlotComponentBase plot)
        {
            if (!currentMap.TryGetValue(plot.story, out var plotName)) return false;
            return plot.plotName == plotName;
        }
    }
}