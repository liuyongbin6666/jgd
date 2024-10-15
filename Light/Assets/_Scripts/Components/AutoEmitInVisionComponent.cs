using UnityEngine;

/// <summary>
/// 自动发射在视野内的组件，用于在视野内发射光线等效果。
/// </summary>
public class AutoEmitInVisionComponent : ColliderComponentBase
{
    [SerializeField] SpriteRenderer renderer;

    protected override void OnCollider3DEnter(Collider col) => SetMaterialEmit( true);
    protected override void OnColliderEnter(Collider2D col) => SetMaterialEmit(true);
    protected override void OnColliderExit(Collider2D col) => SetMaterialEmit(false);
    protected override void OnCollider3DExit(Collider col) => SetMaterialEmit(false);
    // 用于设置材质中布尔值的方法
    void SetMaterialEmit(bool value)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_IsEmit", value ? 1.0f : 0.0f);
        renderer.SetPropertyBlock(mpb);
    }
}
