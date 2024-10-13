using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 游戏操作单位
/// </summary>
public class PlayableUnit : ModelBase
{
    /// <summary>
    /// 灯笼值
    /// </summary>
    public int Lantern { get; private set; }= 1;
    PlayerControlComponent PlayerControl { get; }

    public PlayableUnit(PlayerControlComponent playerControl,int lantern, float lightStep)
    {
        Lantern = lantern;
        PlayerControl = playerControl;
        PlayerControl.Init(lightStep);
        PlayerControl.Lantern(Lantern);
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
        LanternUpdate(--Lantern);
        //Log(nameof(OnLanternTimeout)+$" : {lantern}");
    }

    //当获取到萤火虫
    void OnCollectFirefly()
    {
        LanternUpdate(++Lantern);
        Log(nameof(OnCollectFirefly)+$" lantern: {Lantern}");
    }

    //灯笼更新
    void LanternUpdate(int value)
    {
        Lantern = value;// 灯笼++
        if (Lantern <= 0)
        {
            Lantern = 0;
            PlayerControl.StartPanic();// 开始恐慌
        }
        Log($"value = {value}");
        PlayerControl.Lantern(Lantern);
        SendEvent(GameEvent.Player_Lantern_Update);
    }
    public void Move(Vector3 direction) => PlayerControl.axisMovement = direction.ToVector2();
}