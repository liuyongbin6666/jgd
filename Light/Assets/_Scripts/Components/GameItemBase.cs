using UnityEngine;

/// <summary>
/// 游戏物件基类, 自动调用PlayerControlComponent的GameItemInteraction方法<br/>
/// 之类只需要实现Invoke方法即可
/// </summary>
public abstract class GameItemBase : PlayerTrackingComponentBase,IGameItem
{
    public abstract GameItemType Type { get; }
    /// <summary>
    /// 触发游戏物件
    /// </summary>
    /// <param name="player"></param>
    public abstract void Invoke(PlayableUnit player);

    protected override void OnPlayerTrackingEnter(PlayerControlComponent player) => player.GameItemInteraction(this);
    protected override void OnPlayerTrackingExit(PlayerControlComponent player) {  }
}

/// <summary>
/// 检查玩家进入和离开的组件基类
/// </summary>
public abstract class PlayerTrackingComponentBase : ColliderComponentBase
{
    protected abstract void OnPlayerTrackingEnter(PlayerControlComponent player);
    protected abstract void OnPlayerTrackingExit(PlayerControlComponent player);

    protected override void OnCollider3DEnter(Collider col)
    {
        var player = col.GetControlFromColliderHandler();
        if (player != null) OnPlayerTrackingEnter(player);
    }

    protected override void OnColliderEnter(Collider2D col)
    {
        var player = col.GetControlFromColliderHandler();
        if (player != null) OnPlayerTrackingEnter(player);
    }

    protected override void OnCollider3DExit(Collider col)
    {
        var player = col.GetControlFromColliderHandler();
        if (player != null) OnPlayerTrackingExit(player);
    }

    protected override void OnColliderExit(Collider2D col)
    {
        var player = col.GetControlFromColliderHandler();
        if (player != null) OnPlayerTrackingExit(player);
    }
}