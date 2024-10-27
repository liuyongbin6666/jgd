using System;
using System.Collections.Generic;
using Config;
using GameData;
using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components
{
    /// <summary>
    /// 切换引导传感器，用于切换鬼魂引导的传感器的状态。
    /// </summary>
    public class ChangeGuidingSensor : PlotSensorBase
    {
        [SerializeField, LabelText("引导分支")] GuidePlot[] guides;
        [SerializeField, LabelText("传感器")] SensorSoBase sensorSo;
        protected override SensorSoBase SensorSo => sensorSo;
        protected override SensorManager SensorManager => Game.SensorManager;
        IEnumerable<string> GetPlotNames() => plotComponent.story.GetPlotNames();
        bool IsPlayerReaching { get; set; }
        protected override void OnSensorInit()
        {
            foreach (var guide in guides) guide.RegEnter(CheckIfPlayer);
        }
        void CheckIfPlayer(string plotName,GameObject obj)
        {
            IsPlayerReaching = plotComponent.IsCurrentState() && obj.CompareTag(GameTag.Player);
            plotComponent.PlotManager.OverrideGuide(plotName);
        }

        protected override bool CheckCondition()
        {
            //XArg.Format(new{IsPlayerReaching}).Log(this);
            return IsPlayerReaching;
        }
        [Serializable] class GuidePlot
        {
            [SerializeField, LabelText("引导情节"),ValueDropdown("@((ChangeGuidingSensor)$property.Tree.WeakTargets[0]).GetPlotNames()")] string plotName;
            [SerializeField, LabelText("碰撞处理器")] Collider3DHandler handler;
            public void RegEnter(Action<string, GameObject> onEnterAction) =>
                handler.RegEnter(o => onEnterAction(plotName, o));
        }
    }
}