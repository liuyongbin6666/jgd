using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using EnhancedHierarchy;
using GameData;
using GMVC.Core;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Components
{
    public abstract class PlotComponentBase : GameStartInitializer
    {
        public enum States
        {
            None,
            Begin,
            Finalize,
        }
        protected enum TextMode
        {
            [InspectorName("情节开启")] Begin,
            [InspectorName("情节交互")] Interaction,
            [InspectorName("情节完成")] Finalize,
            [InspectorName("不自动播放")] None,
        }
        [SerializeField, LabelText("文本播放在")] protected TextMode mode;

        public PlotManager PlotManager => Game.PlotManager;
        public States State { get; private set; }
        public bool IsStoryFinalized =>PlotManager.IsStoryFinalized(story);

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
        protected abstract bool DisableLines { get; }
        /// <summary>
        /// 只可以开启一次
        /// </summary>
        public void Begin()
        {
            if (State != States.None) return;
            State = States.Begin;
            if (!DisableLines && mode == TextMode.Begin) SendLines();
            OnBegin();
        }
        /// <summary>
        /// 开始情节，初始化并扩展控件逻辑
        /// </summary>
        protected abstract void OnBegin();

        public void Interaction()
        {
            if (State != States.Begin) return;
            PlotManager.OnInteractPlot(this);
            if (!DisableLines && mode == TextMode.Interaction) SendLines();
            Finalization();
        }
        void Finalization()
        {
            if (State != States.Begin) return;
            OnFinalization();
            State = States.Finalize;
            if (!DisableLines && mode == TextMode.Finalize) SendLines();
            PlotManager.TriggerNext(this);
        }

        protected void SendLines()
        {
            var (type, lines) = story.GetLines(plotName);
            PlotManager.SendLines(type, lines);
        }
        /// <summary>
        /// 当情节结束时触发
        /// </summary>
        protected abstract void OnFinalization();
        public bool IsCurrentState() => PlotManager.IsCurrentPlot(this);
        public virtual void Active(bool active) => this.Display(active);

        public void RegOnNextGuideChange(UnityAction<string> targetPlot) =>
            PlotManager.OnOverrideGuide.AddListener(targetPlot);

        public void RegStoryEnd(UnityAction<StorySo, int> storyEnding) =>
            PlotManager.OnStoryEnd.AddListener(storyEnding);
    }
}