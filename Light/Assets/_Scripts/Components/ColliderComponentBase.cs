using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 碰撞组件基类，处理碰撞并发射事件。可以自由支持触发和碰撞机制。
/// </summary>
public abstract class ColliderComponentBase : MonoBehaviour
{

    [SerializeField] Collider3DHandler _unitCollider3D;
    [SerializeField, ValueDropdown(nameof(GetTags)), LabelText("碰撞目标标签")] string _targetTag;
    static string[] GetTags() => UnityEditorInternal.InternalEditorUtility.tags;

    void Start()
    {
        _unitCollider3D?.OnTriggerEnterEvent.AddListener(Collider3DEnter);
        _unitCollider3D?.OnTriggerExitEvent.AddListener(Collider3DExit);
        _unitCollider3D?.OnCollisionEnterEvent.AddListener(c => Collider3DEnter(c.collider));
        _unitCollider3D?.OnCollisionExitEvent.AddListener(c => Collider3DExit(c.collider));
        OnStart();
    }
    protected virtual void OnStart() { }
    void Collider3DExit(Collider col)
    {
        if (col.gameObject.CompareTag(_targetTag)) OnCollider3DExit(col);
    }

    void ColliderExit(Collider2D col)
    {
        if (col.gameObject.CompareTag(_targetTag)) OnColliderExit(col);
    }

    void Collider3DEnter(Collider col)
    {
        if (col.gameObject.CompareTag(_targetTag))OnCollider3DEnter(col);
    }

    void ColliderEnter(Collider2D col)
    {
        if (col.gameObject.CompareTag(_targetTag)) OnColliderEnter(col);
    }

    protected abstract void OnCollider3DEnter(Collider col);
    protected abstract void OnColliderEnter(Collider2D col);
    protected abstract void OnColliderExit(Collider2D col);
    protected abstract void OnCollider3DExit(Collider col);
}