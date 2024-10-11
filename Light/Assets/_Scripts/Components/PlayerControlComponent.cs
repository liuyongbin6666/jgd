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
    /// <summary>
    /// 摇杆移动方位
    /// </summary>
    public Vector2 AxisMovement { get; private set; }
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
    /// 恐慌, 会一直跳动，直到秒数小或等于0, 会覆盖之前的恐慌状态
    /// </summary>
    /// <param name="onAfterAStep">当每次跳动，并且返回剩余秒数</param>
    /// <param name="totalSecs">总共秒数</param>
    /// <param name="stepSecs">跳动秒数</param>
    public void StartPanic(Action<float> onAfterAStep,float totalSecs = 5f,float stepSecs = 1f)
    {
        StopCoroutine(PanicCoroutine());
        StartCoroutine(PanicCoroutine());
        return;

        IEnumerator PanicCoroutine()
        {
            IsPanic = true;
            var sec = totalSecs;
            while (sec > 0)
            {
                yield return new WaitForSeconds(stepSecs);
                sec--;
                onAfterAStep?.Invoke(sec);
            }
        }
    }
    public void StopPanic() => StopAllCoroutines();//暂时这样停止，实际上会停止所有协程。
    public void AddLightRadius(float radius) => _lightVision.AddOuterRadius(radius);
    public void SetLightRadius(float radius) => _lightVision.SetOuterRadius(radius);
    void Update()
    {
        AxisMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    void FixedUpdate()
    {
        // 使用 MovePosition 进行物理移动，这样可以确保碰撞检测正常
        rb.MovePosition(rb.position + AxisMovement * moveSpeed * Time.fixedDeltaTime);
    }
}