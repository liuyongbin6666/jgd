using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 视野管理控件
/// </summary>
public class LightVisionComponent : MonoBehaviour
{
    [SerializeField] RederMode _mode;

    [SerializeField, HideIf(nameof(_mode), RederMode.M_2D)] Light light3D;
    [SerializeField, HideIf(nameof(_mode), RederMode.M_3D)] Light2D playerLight;
    [SerializeField,LabelText("最大范围")] float maxLightOuterRadius = 2.5f; // 光的最大范围
    [SerializeField,LabelText("最小范围")] float minLightOuterRadius = 0.5f;  // 光的最小范围
    public void Init()
    {
    }
    [Button("设置最大光圈")] public void SetMaxLight() => SetOuterRadius(maxLightOuterRadius);
    public void AddOuterRadius(float radius) => SetOuterRadius(GetOuterRadius() + radius);
    public void SetOuterRadius(float radius)
    {
        switch (_mode)
        {
            case RederMode.M_2D:
                playerLight.pointLightOuterRadius = Mathf.Clamp(
                    radius, minLightOuterRadius, maxLightOuterRadius); // 每个萤火虫增加视野范围
                break;
            case RederMode.M_3D:
                light3D.range = Mathf.Clamp(
                    radius, minLightOuterRadius, maxLightOuterRadius); // 每个萤火虫增加视野范围
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    float GetOuterRadius() =>
        _mode switch
        {
            RederMode.M_2D => playerLight.pointLightOuterRadius,
            RederMode.M_3D => light3D.range,
            _ => throw new ArgumentOutOfRangeException()
        };
    public Collider2D[] CheckForEnemiesInView(LayerMask detectLayer)
    {
        // 检测视野范围内的碰撞器
        return Physics2D.OverlapCircleAll(transform.position, GetOuterRadius(), detectLayer);
    }
}