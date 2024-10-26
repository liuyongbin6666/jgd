using Components;
using GameData;
using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SoulComponent : GameItemBase
{
    [LabelText("故事")] public PlotComponentBase PlotComponent;
    [ReadOnly]public Transform playerTransform;
    [ReadOnly]public Transform targetPlotTransform; // 下一个故事点的 Transform
    [LabelText("环绕半径")]public float orbitRadius = 2f; // 环绕半径
    [LabelText("环绕速度/秒")]public float orbitSpeed = 30f; // 环绕速度（角速度，度/秒）
    [LabelText("环绕角度随机范围")]public float angleOffsetRange = 30f; // 环绕角度的随机偏移范围
    [LabelText("朝向目标方向的偏向程度"),Range(0.5f,1)]public float directionBias = 0.9f; // 朝向目标方向的偏向程度（0-1）

    public override GameItemType Type => GameItemType.Soul;

    float currentAngle;
    bool isGuiding = false;

    public override void Invoke(PlayableUnit player)
    {
        playerTransform = player.PlayerControl.transform;
        StartCoroutine(StoryGuide());
    }

    IEnumerator StoryGuide()
    {
        var story = PlotComponent.story;
        var currentPlotName = PlotComponent.plotName;
        PlotComponentBase[] nextPlots;

        while (!string.IsNullOrWhiteSpace(currentPlotName) || !story.IsStoryEnd(currentPlotName))
        {
            nextPlots = PlotComponent.PlotManager.GetNextPlots(story);
            var nextPlot = nextPlots.FirstOrDefault();

            if (nextPlot != null)
            {
                targetPlotTransform = nextPlot.transform;
                isGuiding = true;

                // 计算灵魂导游与玩家和目标位置的初始角度
                var toTarget = targetPlotTransform.position - playerTransform.position;
                var targetAngle = Mathf.Atan2(toTarget.z, toTarget.x) * Mathf.Rad2Deg;

                // 在目标方向附近随机一个角度偏移量
                var angleOffset = Random.Range(-angleOffsetRange, angleOffsetRange);
                currentAngle = targetAngle + angleOffset;

                // 开始引导
                while (nextPlot.State != PlotComponentBase.States.Finalize)
                {
                    OrbitTowardsTarget();
                    yield return null;
                }

                isGuiding = false;
            }

            currentPlotName = nextPlot?.plotName ?? string.Empty;
            yield return null;
        }
    }

    void OrbitTowardsTarget()
    {
        // 更新角度，使灵魂导游围绕玩家旋转，并逐渐朝向目标方向
        var targetAngle = GetAngleToTarget();
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, directionBias * Time.deltaTime);//逐渐将 currentAngle 插值到 targetAngle，控制灵魂导游朝向目标方向的速度。
        currentAngle += orbitSpeed * Time.deltaTime; //添加环绕的角速度，使灵魂导游绕着玩家旋转。
        currentAngle %= 360f; // 确保角度在 0-360 度之间

        // 计算灵魂导游在圆周上的位置
        var radian = currentAngle * Mathf.Deg2Rad;
        var x = playerTransform.position.x + orbitRadius * Mathf.Cos(radian);
        var z = playerTransform.position.z + orbitRadius * Mathf.Sin(radian);
        var orbitPosition = new Vector3(x, transform.position.y, z);

        // 设置灵魂导游的位置
        transform.position = orbitPosition;
    }
    // 计算玩家到目标位置的角度。
    float GetAngleToTarget()
    {
        if (targetPlotTransform == null)
            return currentAngle;

        var toTarget = targetPlotTransform.position - playerTransform.position;
        var angle = Mathf.Atan2(toTarget.z, toTarget.x) * Mathf.Rad2Deg;
        return angle;
    }
}
