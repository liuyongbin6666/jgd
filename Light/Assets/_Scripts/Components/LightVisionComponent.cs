using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 视野管理控件
/// </summary>
public class LightVisionComponent : MonoBehaviour
{
    [SerializeField] Light light3D;
    [SerializeField,LabelText("最大范围")] float maxLightOuterRadius = 2.5f; // 光的最大范围
    [SerializeField,LabelText("最小范围")] float minLightOuterRadius = 0.5f;  // 光的最小范围
    public void Init()
    {
    }
    [Button("设置最大光圈")] public void SetMaxLight() => SetOuterRadius(maxLightOuterRadius);
    public void AddOuterRadius(float radius) => SetOuterRadius(GetOuterRadius() + radius);
    public void SetOuterRadius(float radius) =>
        light3D.range = Mathf.Clamp(radius, minLightOuterRadius, maxLightOuterRadius); // 每个萤火虫增加视野范围

    float GetOuterRadius() => light3D.range;
    public Collider[] CheckForEnemiesInView(LayerMask detectLayer)
    {
        // 检测视野范围内的碰撞器
        return Physics.OverlapSphere(transform.position, GetOuterRadius(), detectLayer);
    }
}