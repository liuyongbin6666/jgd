using Components;
using GameData;
using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SoulComponent : GameItemBase
{
    [LabelText("故事")] public PlotComponentBase PlotComponent;
    public Transform playerTransform;
    public Transform targetPlotTransform; // 下一个故事点的 Transform
    public float orbitRadius = 3f; // 环绕半径
    public float orbitSpeed = 30f; // 环绕速度（角速度，度/秒）
    public float moveSpeed = 2f; // 朝向目标移动的速度
    public float angleOffsetRange = 30f; // 环绕角度的随机偏移范围
    public float directionBias = 0.7f; // 朝向目标方向的偏向程度（0-1）

    public override GameItemType Type => GameItemType.Soul;

    private float currentAngle;
    private bool isGuiding = false;

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

        while (!story.IsStoryEnd(currentPlotName))
        {
            nextPlots = PlotComponent.PlotManager.GetNextPlots(story);
            var nextPlot = nextPlots.FirstOrDefault();

            if (nextPlot != null)
            {
                targetPlotTransform = nextPlot.transform;
                isGuiding = true;

                // 计算灵魂导游与玩家和目标位置的初始角度
                Vector3 toTarget = playerTransform.position - targetPlotTransform.position;
                float targetAngle = Mathf.Atan2(toTarget.z, toTarget.x) * Mathf.Rad2Deg;

                // 在目标方向附近随机一个角度偏移量
                float angleOffset = Random.Range(-angleOffsetRange, angleOffsetRange);
                currentAngle = targetAngle + angleOffset;

                // 开始引导
                while (!nextPlot.IsFinalized)
                {
                    OrbitTowardsTarget();
                    yield return null;
                }

                isGuiding = false;
            }
            yield return null;
        }
    }

    void OrbitTowardsTarget()
    {
        // 更新角度，使灵魂导游围绕玩家旋转，并逐渐朝向目标方向
        float targetAngle = GetAngleToTarget();
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, directionBias * Time.deltaTime);
        currentAngle += orbitSpeed * Time.deltaTime;
        currentAngle %= 360f; // 确保角度在 0-360 度之间

        // 计算灵魂导游在圆周上的位置
        float radian = currentAngle * Mathf.Deg2Rad;
        float x = playerTransform.position.x + orbitRadius * Mathf.Cos(radian);
        float z = playerTransform.position.z + orbitRadius * Mathf.Sin(radian);
        Vector3 orbitPosition = new Vector3(x, transform.position.y, z);

        // 设置灵魂导游的位置
        transform.position = orbitPosition;
    }

    float GetAngleToTarget()
    {
        if (targetPlotTransform == null)
            return currentAngle;

        Vector3 toTarget = targetPlotTransform.position - playerTransform.position;
        float angle = Mathf.Atan2(toTarget.z, toTarget.x) * Mathf.Rad2Deg;
        return angle;
    }
}
