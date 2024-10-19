using GMVC.Core;
using MyBox;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
/// 环境控件，主要控制闪雷
/// </summary>
public class EnvironmentComponent : CountdownComponent
{
    [SerializeField] GameLaunch _gameLaunch;
    protected override int PulseTimes { get; } = 1;
    protected override float Duration => _duration;
    [SerializeField] Light directionalLight; // 3d全局方向光照组件
    [SerializeField, LabelText("闪电随机范围值")] MinMaxFloat _lightningRange;
    [SerializeField, LabelText("闪电时的光强度")] float lightningIntensity = 0.15f;
    [SerializeField, LabelText("闪电持续时间")] float lightningInterval = 0.15f;
    float _duration = 3;

    public void Init()
    {
        OnCountdownComplete.AddListener(Lightning);
        RandomDuration();
        StartCountdown(true);
    }

    void RandomDuration() => _duration = Random.Range(_lightningRange.Min, _lightningRange.Max);

    void Lightning()
    {
        if (Game.World.Status != GameWorld.GameStates.Playing) return;
        RandomDuration();
        var times = Random.Range(2, 4);
        StartCoroutine(LightningEffect(times, lightningInterval, StartCountdown));
    }

    IEnumerator LightningEffect(int times, float interval, UnityAction onLightningFinish)
    {
        // 保存原始光强度
        float originalIntensity = GetLightIntensity();
        var t = times;
        Game.SendEvent(GameEvent.Env_Lightning);
        while (t > 0)
        {
            yield return Lightning();
            t--;
        }
        onLightningFinish?.Invoke();
        yield break;

        IEnumerator Lightning()
        {
            // 设置全局光为闪电的光强度
            SetIntensity(lightningIntensity);
            // 闪电持续时间
            yield return new WaitForSeconds(interval);
            // 恢复原始光强度
            SetIntensity(originalIntensity);
            yield return new WaitForSeconds(interval / 3f);
        }
    }

    void SetIntensity(float intensity) => directionalLight.intensity = intensity;
    float GetLightIntensity() => directionalLight.intensity;
}