using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 怪物死亡传感器，
/// </summary>
public class BossDeathTrackingSensor : PlotSensorBase
{
    [SerializeField, LabelText("传感器")] SensorSoBase sensorSo;
    [SerializeField, LabelText("Boss")] BossComponent boss;
    protected override SensorSoBase SensorSo => sensorSo;
    protected override SensorManager SensorManager => Game.SensorManager;
    protected override void OnInit(){}
    protected override bool CheckCondition() => boss.IsDeath;
}