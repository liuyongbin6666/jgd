using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 灯笼控件
/// </summary>
public class LanternComponent : CountdownComponent
{
    [SerializeField,LabelText("减弱时长")] float _lastingPerFirefly = 3f;
    [SerializeField] LightVisionComponent _light;
    [SerializeField] VisionLevelComponent _visionLevel;
    protected override int PulseTimes => 1;
    protected override float Duration => _lastingPerFirefly;
    public float VisionRadius => _visionLevel.VisionRadius;
    public void Init()
    {
        _light.Init();
        StartCountdown();
    }
    public void SetVision(float radius) => _light.SetOuterRadius(radius);
    public void AddVision(float radius) => _light.AddOuterRadius(radius);

    public void SetVisionLevel(int level)
    {
        _visionLevel.LoadLightLevel(level,out var isMaxLevel);
        if (isMaxLevel) StartCountdown(true);
    }

    public Collider[] CheckForEnemiesInView(LayerMask layer) => _light.CheckForEnemiesInView(layer);
}