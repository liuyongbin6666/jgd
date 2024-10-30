using Components;
using GameData;
using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using Config;
using UnityEngine;
using DG.Tweening;
using GMVC.Core;
using GMVC.Utls;

public class SoulComponent : GameItemBase
{
    [LabelText("故事")] public PlotComponentBase PlotComponent;
    public Renderer Renderer;
    [ReadOnly] public Transform playerTransform;
    [ReadOnly] public Transform targetPlotTransform; // 当前目标情节的 Transform
    [LabelText("环绕半径")] public float orbitRadius = 2f; // 环绕半径
    [LabelText("环绕速度/秒")] public float orbitSpeed = 30f; // 环绕速度（角速度，度/秒）
    [LabelText("环绕角度随机范围")] public float angleOffsetRange = 30f; // 环绕角度的随机偏移范围
    [LabelText("朝向目标方向的偏向程度"), Range(0.5f, 1)] public float directionBias = 0.9f; // 朝向目标方向的偏向程度（0-1）
    public bool InteractionDisable;

    public override GameItemType Type => GameItemType.Soul;

    float currentAngle;
    bool isGuiding = false;
    string overridePlotName = null; // 用于存储外部指定的情节名称
    [ReadOnly] public PlotComponentBase guidePlot; // 当前引导的情节

    bool isAscending = false;
    bool isPaused = false; // 指示组件是否暂停
    bool isActive = true; // 指示组件是否激活
    protected override void OnGameInit()
    {
        base.OnGameInit();
        PlotComponent.OnActiveEvent.AddListener(Activate);
    }

    public override void Invoke(PlayableUnit player)
    {
        if (!isActive)
        {
            // 如果组件未激活，向玩家提供反馈
            OnInactiveInteraction();
            return;
        }

        if (InteractionDisable) return;
        InteractionDisable = true;

        // 如果组件已暂停，向玩家提供反馈
        if (isPaused)
        {
            OnPausedInteraction();
            InteractionDisable = false;
            return;
        }
        
        // 正常的交互逻辑
        PlotComponent.RegOnNextGuideChange(GuideChange);
        PlotComponent.RegStoryEnd(StoryEnding);
        playerTransform = player.PlayerControl.transform;
        StartCoroutine(StoryGuide());
        Game.SendEvent(GameEvent.Story_Soul_Interactive);
    }

    void StoryEnding(StorySo story, int endingCode)
    {
        if (story != PlotComponent.story) return;
        var isBadEnding = endingCode != 0;
        if (isBadEnding)
        {
            this.Display(false);
            return;
        }
        AscendToHeaven();
    }

    IEnumerator StoryGuide()
    {
        var story = PlotComponent.story;
        var currentPlotName = PlotComponent.plotName;
        PlotComponentBase[] nextPlots;

        while (!string.IsNullOrWhiteSpace(currentPlotName) || !story.IsStoryEnd(currentPlotName))
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            // 检查是否有外部指定的情节
            if (!string.IsNullOrEmpty(overridePlotName))
            {
                // 使用指定的情节作为目标
                guidePlot = PlotComponent.PlotManager.FindPlotByName(story, overridePlotName);
                overridePlotName = null; // 重置变量
            }
            else
            {
                // 获取下一个情节
                nextPlots = GetNextPlots(story);
                guidePlot = nextPlots.FirstOrDefault();
            }

            if (guidePlot != null)
            {
                targetPlotTransform = guidePlot.transform;
                isGuiding = true;

                // 计算初始角度
                var toTarget = targetPlotTransform.position - playerTransform.position;
                var targetAngle = Mathf.Atan2(toTarget.z, toTarget.x) * Mathf.Rad2Deg;

                // 在目标方向附近随机一个角度偏移量
                var angleOffset = Random.Range(-angleOffsetRange, angleOffsetRange);
                currentAngle = targetAngle + angleOffset;

                // 开始引导
                while (guidePlot is not { State: PlotComponentBase.States.Finalize })
                {
                    if (isPaused)
                    {
                        yield return null;
                        continue;
                    }

                    // 检查是否有新的外部指定情节
                    if (!string.IsNullOrEmpty(overridePlotName))
                    {
                        // 退出当前引导，重新开始循环以更新目标
                        break;
                    }

                    OrbitTowardsTarget();
                    yield return null;
                }

                isGuiding = false;

                // 更新当前情节名称
                currentPlotName = guidePlot.plotName;
            }
            else
            {
                // 没有找到下一个情节，跳出循环
                break;
            }

            yield return null;
        }

        // 在引导流程结束后，触发升天效果
        AscendToHeaven();
    }

    PlotComponentBase[] GetNextPlots(StorySo story)
    {
        var currents = PlotComponent.PlotManager.GetCurrentPlotNames(story);
        var names = currents.SelectMany(story.NextPlots).ToList();
        var possibleNext = PlotComponent.PlotManager.FindPlots(story, names);
        return possibleNext.ToArray();
    }

    void OrbitTowardsTarget()
    {
        if (playerTransform == null || targetPlotTransform == null || isAscending || isPaused)
            return;

        // 更新角度，使灵魂导游围绕玩家旋转，并逐渐朝向目标方向
        var targetAngle = GetAngleToTarget();
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, directionBias * Time.deltaTime);
        currentAngle += orbitSpeed * Time.deltaTime;
        currentAngle %= 360f; // 确保角度在 0-360 度之间

        // 计算灵魂导游在圆周上的位置
        var radian = currentAngle * Mathf.Deg2Rad;
        var x = playerTransform.position.x + orbitRadius * Mathf.Cos(radian);
        var z = playerTransform.position.z + orbitRadius * Mathf.Sin(radian);
        var orbitPosition = new Vector3(x, transform.position.y, z);

        // 设置灵魂导游的位置
        transform.position = orbitPosition;
    }

    // 计算玩家到目标位置的角度
    float GetAngleToTarget()
    {
        if (targetPlotTransform == null || playerTransform == null)
            return currentAngle;

        var toTarget = targetPlotTransform.position - playerTransform.position;
        var angle = Mathf.Atan2(toTarget.z, toTarget.x) * Mathf.Rad2Deg;
        return angle;
    }

    // 设置新的目标情节
    public void GuideChange(string plotName) => overridePlotName = plotName;

    // 停止引导
    public void StopGuiding()
    {
        overridePlotName = null;
        isGuiding = false;
        targetPlotTransform = null;
        guidePlot = null;
    }

    // 添加魂魄升天的功能
    void AscendToHeaven()
    {
        if (isAscending) return; // 防止重复调用
        isAscending = true;

        // 停止引导
        StopGuiding();

        // 设置升天动画持续时间
        var duration = 4f;

        // 上升的距离
        var ascentHeight = 3f;

        // 上升动画
        transform.DOMoveY(transform.position.y + ascentHeight, duration).SetEase(Ease.OutCubic);

        // 淡出动画
        if (Renderer)
        {
            // 获取初始 Alpha 值
            var initialAlpha = Renderer.material.GetFloat("_Alpha");
            // 开始渐变动画
            DOTween.To(() => initialAlpha, x => {
                initialAlpha = x;
                Renderer.material.SetFloat("_Alpha", initialAlpha);
            }, 0f, duration).OnComplete(() => {
                // 动画完成后，隐藏对象
                this.Display(false);
            });
        }
        else
        {
            // 如果未找到材质，直接在动画完成后隐藏对象
            DOVirtual.DelayedCall(duration, () => {
                this.Display(false);
            });
        }
    }

    // 暂停引导功能
    public void Pause(bool pause) => isPaused = pause;

    // 启用组件
    public void Activate(bool active) => isActive = active;

    // 当组件未激活时的交互
    void OnInactiveInteraction()
    {
        // 向玩家提供反馈，例如显示一条消息
        Game.SendEvent(GameEvent.Story_Soul_Inactive, PlotComponent.story.DisableMessage);
    }

    // 当组件暂停时的交互
    void OnPausedInteraction()
    {
        //Debug.Log("这个魂魄目前处于休眠状态，请先完成其他故事。");
    }
}
