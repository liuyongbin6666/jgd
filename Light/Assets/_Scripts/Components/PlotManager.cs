using System.Collections.Generic;
using System.Linq;
using Config;
using GameData;
using GMVC.Utls;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Components
{
    /// <summary>
    /// 情节管理器, 注册并处理所有情节事件，根据情节的配置调用场景中的组件
    /// </summary>
    public class PlotManager : MonoBehaviour
    {
        readonly Dictionary<StorySo, List<PlotComponentBase>> data = new();
        public readonly UnityEvent<StageStory.Lines, string[]> OnLinesEvent = new();

        public readonly UnityEvent<StorySo,int> OnStoryEnd = new();
        //public readonly UnityEvent<PlotComponentBase> OnPlotBegin = new();
        Dictionary<StorySo, List<string>> currentMap = new();//当前情节,如果剧情结束是空的
        public readonly UnityEvent<string> OnOverrideGuide = new();
        public void Init(StorySo[] stories)
        {
            foreach (var story in stories)
            {
                data.Add(story,new List<PlotComponentBase>());
                currentMap.Add(story,new List<string> { story.GetFirstPlot() });//第一个情节自动为当前情节
            }
        }

        public void RegComponent(PlotComponentBase plot)
        {
            if(!data.ContainsKey(plot.story)) data.Add(plot.story, new List<PlotComponentBase>());
            data[plot.story].Add(plot);
        }
        public void UnRegComponent(PlotComponentBase plot)
        {
            if (data.TryGetValue(plot.story, out var com)) com.Remove(plot);
        }
        public void TriggerNext(PlotComponentBase com)
        {
            var story = com.story;
            if (!data.TryGetValue(story, out var list)) return;
            var currentFinish = com.plotName;
            var storyEnd = story.IsStoryEnd(currentFinish);
            com.Active(false);
            if (storyEnd)
            {
                var code = story.GetEndingCode(currentFinish);
                foreach (var plot in list) plot.Active(false);
                currentMap[story].Clear();//剧情结束，清空当前情节
                OnStoryEnd.Invoke(story, code);
                return;
            }
            //移除当前情节
            currentMap[story].Remove(com.plotName);
            //获取当前仍未结束的情节
            var activePlots = GetActivePlots(story).ToList();
            var nextPlotNames = story.NextPlots(currentFinish);//获取下一个情节
            var nextPlots = GetFromData(story, nextPlotNames);
            activePlots.AddRange(nextPlots);//合并情节
            //string.Join(',',activePlots.Select(p=>p.plotName)).Log(this);
            currentMap[story] = activePlots.Select(p=>p.plotName).ToList();
            foreach (var plot in activePlots)
            {
                plot.Active(true);
                plot.Begin();
            }
        }

        public void OnInteractPlot(PlotComponentBase plot)
        {
            var story = plot.story;
            DisablePlots(story.GetDisablePlots(plot.plotName));

            void DisablePlots(IEnumerable<string> disablePlots)
            {
                //获取需要禁用的情节
                var disableList = GetFromData(plot.story, disablePlots);
                //禁用情节
                foreach (var disable in disableList) disable.Active(false);
            }
        }

        PlotComponentBase[] GetFromData(StorySo story, IEnumerable<string> plotNames) =>
            data[story].Join(plotNames, p => p.plotName, n => n, (p, _) => p).ToArray();

        IEnumerable<PlotComponentBase> GetActivePlots(StorySo story)
        {
            return currentMap[story]
                .Join(data[story], n => n, c => c.plotName, (_, c) => c)
                .Where(p => p.gameObject.activeSelf);
        }

        public void SendLines(StageStory.Lines type, string[] lines) => OnLinesEvent?.Invoke(type, lines);
        //public void SetCurrentPlot(PlotComponentBase plot)
        //{
        //    if (plot) currentMap[plot.story] = plot.plotName;
        //    OnPlotBegin.Invoke(plot);
        //}

        public bool IsCurrentPlot(PlotComponentBase plot)
        {
            if (!currentMap.TryGetValue(plot.story, out var current)) return false;
            return current.Contains(plot.plotName);
        }
        public IEnumerable<PlotComponentBase> FindPlots(StorySo story, IEnumerable<string> names) =>
            data[story].Join(names, p => p.plotName, n => n, (p, _) => p);

        public IEnumerable<string> GetCurrentPlotNames(StorySo story) => currentMap[story];
        public bool IsStoryFinalized(StorySo story)=> currentMap[story] is null;

        public PlotComponentBase FindPlotByName(StorySo story, string plotName) =>
            GetFromData(story, new[] { plotName }).FirstOrDefault();

        public void OverrideGuide(string plotName) => OnOverrideGuide.Invoke(plotName);
    }
}