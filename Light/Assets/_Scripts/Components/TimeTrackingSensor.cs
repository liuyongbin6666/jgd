using Config;
using GMVC.Core;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace Components
{
    /// <summary>
    /// 倒数传感器, 用于倒数计时后执行
    /// </summary>
    public class TimeTrackingSensor : PlotSensorBase
    {
        [SerializeField, LabelText("传感器")] SensorSoBase sensorSo;
        [SerializeField, LabelText("读秒")] float timeout = 1f;
        protected override SensorSoBase SensorSo => sensorSo;
        protected override SensorManager SensorManager => Game.SensorManager;
        bool isInit;
        bool isDone;
        protected override void OnGameStart()
        {
            base.OnGameStart();
            gameObject.Display(false);//隐藏，因为触发机制基于Start机制，读秒触发
            isInit = true;
        }
        protected override void OnSensorInit() => StartCount();
        protected override void OnStart() => StartCount();
        void StartCount()
        {
            if(!isInit)return;
            Invoke(nameof(OnTimeout), timeout);
            this.Log();
        }
        void OnTimeout() => isDone = true;

        protected override bool CheckCondition()
        {
            //XArg.Format(new{isDone}).Log(this);
            return isDone;
        }
    }
}