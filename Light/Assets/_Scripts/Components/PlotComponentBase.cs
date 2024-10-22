using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using GameData;
using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace Components
{
    public abstract class PlotComponentBase : GameStartInitializer
    {
        public PlotManager PlotManager => Game.PlotManager;
        [LabelText("故事")]public StorySo story;
        [LabelText("情节"),ValueDropdown(nameof(GetPlots)),OnValueChanged(nameof(ChangeName))]public string plotName;
        IEnumerable<string> GetPlots() => story?.GetPlotNames() ?? new []{ "请设置剧情" };
        void ChangeName() => name = "情节_" + plotName;

        protected override void OnGameStart()
        {
            PlotManager.RegComponent(this);
            if (story.IsAutoBegin(plotName))
            {
                if (!gameObject.activeSelf) throw new InvalidOperationException($"{name}:自动开启情节必须保证控件是活跃状态！");
                StartCoroutine(BeginRoutine());

                IEnumerator BeginRoutine()
                {
                    yield return new WaitForSeconds(0.1f);
                    OnBegin();
                }
            }
        }

        public void Begin()
        {
            OnBegin();
            PlotManager.SetCurrentPlot(this);
        }
        /// <summary>
        /// 开始情节，初始化并扩展控件逻辑
        /// </summary>
        protected abstract void OnBegin();

        /// <summary>
        /// 结束情节，并触发下一个情节
        /// </summary>
        public void Finalization()
        {
            OnFinalization();
            PlotManager.TriggerNext(this);
        }
        public void SendLines()
        {
            var (type, lines) = story.GetLines(plotName);
            $"{plotName}--文本播放！".Log(this);
            PlotManager.SendLines(type, lines);
        }
        /// <summary>
        /// 当情节结束时触发
        /// </summary>
        protected abstract void OnFinalization();
        public bool IsCurrentState() => PlotManager.IsCurrentPlot(this);
    }
}