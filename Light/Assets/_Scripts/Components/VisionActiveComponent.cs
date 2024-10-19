using UnityEngine;

/// <summary>
/// 自动发射在视野内的组件，用于在视野内发射光线等效果。
/// </summary>
public class VisionActiveComponent : ColliderComponentBase
{
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] Collider[] colliders;

    protected override void OnCollider3DEnter(Collider col) => OnTrigger(true);
    protected override void OnColliderEnter(Collider2D col) => OnTrigger(true);
    protected override void OnColliderExit(Collider2D col) => OnTrigger(false);
    protected override void OnCollider3DExit(Collider col) => OnTrigger(false);

    void OnTrigger(bool enable)
    {
        foreach (var c in colliders) c.enabled = enable;
        SetMaterialEmit(enable);
    }
    // 用于设置材质中布尔值的方法
    void SetMaterialEmit(bool value)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_IsEmit", value ? 1.0f : 0.0f);
        renderer.SetPropertyBlock(mpb);
    }
}