using Config;
using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace Components
{
    /// <summary>
    /// 怪物死亡传感器<br/>
    /// </summary>
    public class BossDeathTrackingSensor : PlotSensorBase
    {
        [SerializeField, LabelText("传感器")] SensorSoBase sensorSo;
        [SerializeField, LabelText("Boss控件")] StoryBossComponent storyBoss;
        protected override SensorSoBase SensorSo => sensorSo;
        protected override SensorManager SensorManager => Game.SensorManager;
        protected override void OnSensorInit(){}
        protected override bool CheckCondition()
        {
            //XArg.Format(new{boss.IsDeath}).Log(this);
            return !storyBoss || storyBoss.IsUnityNull() || storyBoss.IsDeath;
        }
    }
}