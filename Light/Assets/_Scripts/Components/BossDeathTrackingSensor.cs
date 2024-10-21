using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

/// <summary>
/// 怪物死亡传感器，
/// </summary>
public class BossDeathTrackingSensor : PlotSensorBase
{
    [SerializeField, LabelText("传感器")] SensorSoBase sensorSo;
    [SerializeField, LabelText("Boss")] BossComponent boss;
    protected override SensorSoBase SensorSo => sensorSo;
    protected override SensorManager SensorManager => Game.SensorManager;
    protected override void OnSensorInit(){}

    protected override bool CheckCondition()
    {
        //XArg.Format(new{boss.IsDeath}).Log(this);
        return boss.IsDeath;
    }
}