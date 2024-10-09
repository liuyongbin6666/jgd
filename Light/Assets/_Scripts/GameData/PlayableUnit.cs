/// <summary>
/// 游戏操作单位
/// </summary>
public class PlayableUnit
{
    /// <summary>
    /// 视野/灯笼范围
    /// </summary>
    public float Vision => (Lantern + 1) * 0.5f;
    /// <summary>
    /// 灯笼
    /// </summary>
    public int Lantern { get; private set; } = 1;
    PlayerControlComponent PlayerControl { get; }

    public PlayableUnit(PlayerControlComponent playerControl,int lantern)
    {
        Lantern = lantern;
        PlayerControl = playerControl;
        PlayerControl.SetLightRadius(Vision);
        PlayerControl.OnFireflyCollected.AddListener(OnCollectFirefly);
    }
    //当获取到萤火虫
    void OnCollectFirefly()
    {
        Lantern++;// 灯笼++
        PlayerControl.SetLightRadius(Vision);
    }
}