using UnityEngine;

/// <summary>
/// 游戏物件基类, 自动调用PlayerControlComponent的GameItemInteraction方法<br/>
/// 之类只需要实现Invoke方法即可
/// </summary>
public abstract class GameItemBase : MonoBehaviour,IGameItem
{
    [SerializeField] Collider2DHandler _unitCollider;
    public abstract GameItemType Type { get; }

    /// <summary>
    /// 触发游戏物件
    /// </summary>
    /// <param name="player"></param>
    public abstract void Invoke(PlayableUnit player);

    void Start()
    {
        _unitCollider.OnTriggerEnter.AddListener(OnColliderEnter);
        _unitCollider.OnCollisionEnter.AddListener(c => OnColliderEnter(c.collider));
    }

    void OnColliderEnter(Collider2D col)
    {
        if (!col.CompareTag(GameTag.Player)) return;
        var component = col.GetControlFromColliderHandler();
        component.GameItemInteraction(this);
    }
}