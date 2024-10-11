using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 灯笼控件
/// </summary>
public class LanternComponent : CountdownComponent
{
    [LabelText("减弱时长")] float _lastingPerFirefly = 3f;
    [SerializeField] LightVisionComponent _light;
    protected override int PulseTimes => 1;
    protected override float Duration => _lastingPerFirefly;

    public void Init()
    {
        _light.Init();
        StartCountdown();
    }
    public void SetVision(float radius) => _light.SetOuterRadius(radius);
    public void AddVision(float radius) => _light.AddOuterRadius(radius);
    public Collider2D[] CheckForEnemiesInView(LayerMask layer) => _light.CheckForEnemiesInView(layer);
}