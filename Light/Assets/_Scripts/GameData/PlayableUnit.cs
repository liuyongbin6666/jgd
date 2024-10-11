using Sirenix.OdinInspector;

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
        PlayerControl.SetVision(Vision,false);
        PlayerControl.OnFireflyCollected.AddListener(OnCollectFirefly);
        PlayerControl.OnLanternTimeout.AddListener(OnLanternTimeout);
        PlayerControl.OnPanicFinalize.AddListener(OnScaryFinalized);
        PlayerControl.OnPanicPulse.AddListener(OnPanicPulse);
    }

    //当恐慌心跳, times = 剩余次数
    void OnPanicPulse(int times)
    {
        //Log(times);
        SendEvent(GameEvent.Player_Panic_Pulse, times);
    }
    //当恐慌结束
    void OnScaryFinalized()
    {
        Log();
        SendEvent(GameEvent.Player_Panic_Finalize);
    }
    //当灯笼减弱时间触发
    void OnLanternTimeout()
    {
        LanternUpdate(--lantern);
        //Log(nameof(OnLanternTimeout)+$" : {lantern}");
    }

    //当获取到萤火虫
    void OnCollectFirefly()
    {
        LanternUpdate(++lantern);
        Log(nameof(OnCollectFirefly)+$" lantern: {lantern}");
    }

    //灯笼更新
    void LanternUpdate(int value)
    {
        lantern = value;// 灯笼++
        if (lantern <= 0)
        {
            lantern = 0;
            PlayerControl.StartPanic();// 开始恐慌
        }
        Log($"value = {value}");
        PlayerControl.SetVision(Vision, lantern <= 0);
        SendEvent(GameEvent.Player_Lantern_Update);
    }
}