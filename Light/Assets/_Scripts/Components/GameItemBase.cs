using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 游戏物件基类, 自动调用PlayerControlComponent的GameItemInteraction方法<br/>
/// 之类只需要实现Invoke方法即可
/// </summary>
public abstract class GameItemBase : MonoBehaviour,IGameItem
{
    [SerializeField] RederMode _mode;
    [SerializeField, HideIf(nameof(_mode), RederMode.M_2D)] Collider3DHandler _unitCollider3D;
    [SerializeField, HideIf(nameof(_mode), RederMode.M_3D)] Collider2DHandler _unitCollider;
    public abstract GameItemType Type { get; }

    /// <summary>
    /// 触发游戏物件
    /// </summary>
    /// <param name="player"></param>
    public abstract void Invoke(PlayableUnit player);

    void Start()
    {
        _unitCollider?.OnTriggerEnter.AddListener(OnColliderEnter);
        _unitCollider?.OnCollisionEnter.AddListener(c => OnColliderEnter(c.collider));
        _unitCollider3D?.OnTriggerEnterEvent.AddListener(OnCollider3DEnter);
        _unitCollider3D?.OnCollisionEnterEvent.AddListener(c => OnCollider3DEnter(c.collider));
    }

    void OnCollider3DEnter(Collider col)
    {
        if (!col.CompareTag(GameTag.Player)) return;
        col.GetControlFromColliderHandler().GameItemInteraction(this);
    }

    void OnColliderEnter(Collider2D col)
    {
        if (!col.CompareTag(GameTag.Player)) return;
        col.GetControlFromColliderHandler().GameItemInteraction(this);
    }
}