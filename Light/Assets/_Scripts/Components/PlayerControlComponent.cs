using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControlComponent : MonoBehaviour
{
    [SerializeField, LabelText("移动速度")] float moveSpeed = 5f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] LightVisionComponent _lightVision;
    [SerializeField] Collider2DHandler _unitCollider;
    public float MoveSpeed => moveSpeed;
    public readonly UnityEvent OnFireflyCollected = new();
    public bool IsPanic { get; private set; }

    void Start() => Init();
    public void Init()
    {
        _unitCollider.OnTriggerEnter.AddListener(OnColliderEnter);
        _unitCollider.OnCollisionEnter.AddListener(c => OnColliderEnter(c.collider));
    }
    public void Log() => gameObject.Log();
    void OnColliderEnter(Collider2D collider)
    {
        if (collider.CompareTag(GameTag.FireFly))
        {
            var handler = collider.GetComponent<Collider2DHandler>();
            OnFireFlyInvoke(handler);
        }
    }
    void OnFireFlyInvoke(Collider2DHandler handler)
    {
        OnFireflyCollected?.Invoke();
        //this.Log($"{handler.root.name} collided!");
        Destroy(handler.root); // 收集后销毁萤火虫
    }
    void OnColliderOnView(Collider2D[] colliders)
    {
        //foreach (var collider in colliders)
        //    if (collider.CompareTag(GameTag.FireFly)) 
        //        OnFireFlyInvoke(collider);
    }
    public void SetSpeed(float speed) => moveSpeed = speed;
    /// <summary>
    /// 恐慌
    /// </summary>
    public void RestartPanic(Action onPanicOver,float delaySecs = 5f)
    {
        StopCoroutine(PanicCoroutine());
        StartCoroutine(PanicCoroutine());
        return;

        IEnumerator PanicCoroutine()
        {
            IsPanic = true;
            yield return new WaitForSeconds(delaySecs);
            onPanicOver?.Invoke();
        }
    }
    public void StopPanic() => StopAllCoroutines();//暂时这样停止，实际上会停止所有协程。
    public void AddLightRadius(float radius) => _lightVision.AddOuterRadius(radius);
    public void SetLightRadius(float radius) => _lightVision.SetOuterRadius(radius);
    void Update()
    {
        var movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.position = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
    }
}