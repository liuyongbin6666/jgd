using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 视野管理控件
/// </summary>
public class LightVisionComponent : MonoBehaviour
{
    [SerializeField] Light2D playerLight;

    [SerializeField,LabelText("减弱速率")] float lightDecreaseRate = 0.02f; // 光的减弱速率
    //[SerializeField,LabelText("最大亮度")] float maxLightIntensity = 1f; // 光的最大亮度
    //[SerializeField,LabelText("最小亮度")] float minLightIntensity = 0.12f; // 光的最小亮度
    [SerializeField,LabelText("最大范围")] float maxLightOuterRadius = 2.5f; // 光的最大范围
    [SerializeField,LabelText("最小范围")] float minLightOuterRadius = 0.5f;  // 光的最小范围
    [SerializeField,LabelText("检测层")] LayerMask detectLayer;
    public void Init()
    {
    }
    [Button("设置最大光圈")] public void SetMaxLight() => SetOuterRadius(maxLightOuterRadius);
    public void AddOuterRadius(float radius)
    {
        //playerLight.intensity = Mathf.Min(maxLightIntensity, intensity); // 每个萤火虫增加光的亮度
        SetOuterRadius(playerLight.pointLightOuterRadius + radius);
    }
    public void SetOuterRadius(float radius)
    {
        playerLight.pointLightOuterRadius = Mathf.Clamp(
            radius, minLightOuterRadius, maxLightOuterRadius); // 每个萤火虫增加视野范围
    }
    public Collider2D[] CheckForEnemiesInView()
    {
        // 检测视野范围内的碰撞器
        return Physics2D.OverlapCircleAll(transform.position, playerLight.pointLightOuterRadius, detectLayer);
    }
}