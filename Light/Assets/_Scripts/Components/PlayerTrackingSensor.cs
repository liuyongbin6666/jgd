using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

/// <summary>
/// 玩家情节传感器，主要用于检测玩家是否进入触发器范围
/// </summary>
public class PlayerTrackingSensor : PlotSensorBase
{
    [SerializeField, LabelText("碰撞处理器")] Collider3DHandler handler;
    [SerializeField, LabelText("传感器")] SensorSoBase sensorSo;
    protected override SensorSoBase SensorSo => sensorSo;
    protected override SensorManager SensorManager => Game.SensorManager;
    bool IsPlayerReaching { get; set; }
    protected override void OnSensorInit() => handler.RegEnter(CheckIfPlayer);
    void CheckIfPlayer(GameObject obj) =>
        IsPlayerReaching = plotComponent.IsCurrentState() && obj.CompareTag(GameTag.Player);

    protected override bool CheckCondition()
    {
        //XArg.Format(new{IsPlayerReaching}).Log(this);
        return IsPlayerReaching;
    }
}