using UnityEngine;

/// <summary>
/// 游戏物件基类, 自动调用PlayerControlComponent的GameItemInteraction方法<br/>
/// 之类只需要实现Invoke方法即可
/// </summary>
public abstract class GameItemBase : ColliderComponentBase,IGameItem
{
    public abstract GameItemType Type { get; }
    /// <summary>
    /// 触发游戏物件
    /// </summary>
    /// <param name="player"></param>
    public abstract void Invoke(PlayableUnit player);
    protected override void OnCollider3DEnter(Collider col) => col.GetControlFromColliderHandler()?.GameItemInteraction(this);
    protected override void OnColliderEnter(Collider2D col) => col.GetControlFromColliderHandler()?.GameItemInteraction(this);
    protected override void OnCollider3DExit(Collider col) {  }
    protected override void OnColliderExit(Collider2D col) { }
}