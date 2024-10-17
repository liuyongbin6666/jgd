using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class VisionLevelComponent : MonoBehaviour
{
    [SerializeField] Collider _collider;  // 关联的BoxCollider
    [SerializeField] Light _pointLight;         // 关联的Point Light
    [SerializeField] LightSettings minSettings; // 最低值设置
    [SerializeField] LightSettings maxSettings; // 最高值设置
    [SerializeField] List<LightSettings> settings; // 设置数组

    [Button("保存当前-Min")]void SetToMin() => minSettings = CurrentLightSetting();
    [Button("保存当前-Max")]void SetToMax() => maxSettings = CurrentLightSetting();

    LightSettings CurrentLightSetting() =>
        new()
        {
            colliderSize = _collider.transform.localScale.x,
            intensity = _pointLight.intensity,
            range = _pointLight.range,
            lightY = _pointLight.transform.localPosition.y
        };

    [Button("计算灯光等级")]
    void GenerateSettings(int totalLevel)
    {
        if (_collider == null || _pointLight == null)
        {
            Debug.LogError("BoxCollider或Point Light未设置！");
            return;
        }

        settings.Clear();
        for (var i = 0; i < totalLevel; i++)
        {
            // 计算归一化等级值 0 到 1
            var normalizedLevel = (float)i / (totalLevel - 1);

            // 线性插值collider大小、lightY、intensity和range
            var colliderSize = Mathf.Lerp(minSettings.colliderSize, maxSettings.colliderSize, normalizedLevel);
            var intensity = Mathf.Lerp(minSettings.intensity, maxSettings.intensity, normalizedLevel);
            var range = Mathf.Lerp(minSettings.range, maxSettings.range, normalizedLevel);
            var lightY = Mathf.Lerp(minSettings.lightY, maxSettings.lightY, normalizedLevel);

            // 将计算结果存入设置列表
            settings.Add(new LightSettings
            {
                colliderSize = colliderSize,
                intensity = intensity,
                range = range,
                lightY = lightY
            });
        }
    }
    [Button("设置灯光")]public void LoadLightLevel(int level, out bool isMaxLevel)
    {
        var maxLevel = settings.Count;
        isMaxLevel = level == maxLevel;
        if (level < 0 || level > maxLevel)
        {
            Debug.LogError($"等级超出范围 1~{maxLevel}！",this);
            return;
        }
        var index = Mathf.Clamp(level, 0, maxLevel - 1);
        var setting = settings[index];
        var intensity = setting.intensity;
        var range = setting.range;
        _collider.transform.localScale
            = new Vector3(setting.colliderSize, _collider.transform.localScale.y, setting.colliderSize);
        // 设置Point Light的参数
        _pointLight.intensity = intensity;
        _pointLight.range = range;
        _pointLight.transform.localPosition = new Vector3(_pointLight.transform.localPosition.x, setting.lightY, _pointLight.transform.localPosition.z);
        Debug.Log($"当前等级: {index+1}, Intensity: {intensity}, Range: {range}");
    }
}

[Serializable]
public class LightSettings
{
    public float colliderSize; // 用于管理collider的大小 (x 或 z)
    public float lightY; // 光源的高度y轴
    public float intensity;    // 光源的强度
    public float range;        // 光源的范围
}