using System;
using System.Collections.Generic;
using System.Linq;
using GameData;
using GMVC.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Config
{
    public abstract class PlotSoBase : AutoNameSoBase
    {
        [OnValueChanged(nameof(RegStory))]public StorySo story;

        [LabelText("故事结束")]public bool isStoryFinalize;
        protected override string Prefix { get; } = "情节-";
        [LabelText("自动开启")]public bool autoBegin;
        [LabelText("文本类型")] public StageStory.Lines lineType;
        [TextArea,LabelText("台词")] public string[] lines;
        [ShowIf(nameof(isStoryFinalize)),LabelText("结局标记，0:一般是好结局,魂魄会升天")]public int endingCode;
        public abstract string[] NextPlots();
        public abstract string[] DisablePlots();
        void RegStory() => story?.RegPlot(this);
        public void RemoveStory() => story = null;
        // 你可以在这里添加更多的配置项，例如触发条件的参数
        public IEnumerable<string> GetPlotNames() => story?.GetPlotNames().Where(n => n != Name) ?? Array.Empty<string>();
    }
}