using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 游戏操作单位
/// </summary>
public class PlayableUnit : ModelBase
{
    /// <summary>
    /// 视野/灯笼范围
    /// </summary>
    public float Vision => (lantern + 1) * lightOuter;

    [LabelText("光圈步进")]float lightOuter = 0.5f;
    [LabelText("灯笼")]public int lantern = 1;
    PlayerControlComponent PlayerControl { get; }

    public PlayableUnit(PlayerControlComponent playerControl,int lantern)
    {
        this.lantern = lantern;
        PlayerControl = playerControl;
        PlayerControl.SetLightRadius(Vision);
        PlayerControl.OnFireflyCollected.AddListener(OnCollectFirefly);
    }
    //当获取到萤火虫
    void OnCollectFirefly() => LanternUpdate(lantern++);
    //灯笼更新
    void LanternUpdate(int lantern)
    {
        this.lantern = lantern;// 灯笼++
        PlayerControl.SetLightRadius(Vision);
        SendEvent(GameEvent.Player_Lantern_Update);
    }
}