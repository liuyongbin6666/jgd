using GMVC.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using static SensorListenerBase;

[CreateAssetMenu(fileName = "触发器", menuName = "配置/触发器")]
public class SensorSoBase : AutoNameSoBase
{
    protected override string Prefix { get; }= "传感器-";
    [LabelText("触发")] public RepeatMode repeat = RepeatMode.Once;
    [LabelText("传感")] [SerializeField] public UpdateStrategy updateStrategy;
}