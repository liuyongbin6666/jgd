using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControlComponent : MonoBehaviour
{
    [SerializeField, LabelText("移动速度")] float moveSpeed = 5f;
    [SerializeField] Rigidbody2D rb;
    //[SerializeField] LightVisionComponent _lightVision;
    [SerializeField] Collider2DHandler _unitCollider;
    [SerializeField] LanternComponent _lantern;
    [SerializeField] PanicComponent _panicCom;
    public readonly UnityEvent OnFireflyCollected = new();
    public readonly UnityEvent OnLanternTimeout = new();
    public readonly UnityEvent OnPanicFinalize = new();
    public readonly UnityEvent<int> OnPanicPulse = new();
    /// <summary>
    /// 摇杆移动方位
    /// </summary>
    public Vector2 AxisMovement { get; private set; }

    void Start() => Init();
    public void Init()
    {
        _lantern.Init();
        _lantern.OnCountdownComplete.AddListener(()=>OnLanternTimeout?.Invoke());
        _panicCom.Init();
        _panicCom.OnPulseTrigger.AddListener(ScaryPulse);
        _unitCollider.OnTriggerEnter.AddListener(ColliderEnter);
        _unitCollider.OnCollisionEnter.AddListener(c => ColliderEnter(c.collider));
    }

    void ColliderEnter(Collider2D collider)
    {
        if (collider.CompareTag(GameTag.FireFly))
        {
            var handler = collider.GetComponent<Collider2DHandler>();
            FireFlyCollect(handler);
        }
    }
    void FireFlyCollect(Collider2DHandler handler)
    {
        OnFireflyCollected?.Invoke();
        //this.Log($"{handler.root.name} collided!");
        Destroy(handler.root); // 收集后销毁萤火虫
    }
    //当恐慌时
    void ScaryPulse(int times)
    {
        if(times == 0)// 0 == 恐慌时间到
        {
            OnPanicFinalize?.Invoke();
            return;
        }
        OnPanicPulse?.Invoke(times);
    }
    void OnColliderOnView(Collider2D[] colliders)
    {
        //foreach (var collider in colliders)
        //    if (collider.CompareTag(GameTag.FireFly)) 
        //        FireFlyCollect(collider);
    }
    public void SetSpeed(float speed) => moveSpeed = speed;
    public void StopPanic() => StopAllCoroutines();//暂时这样停止，实际上会停止所有协程。
    public void SetVision(float radius, bool noVision)
    {
        _lantern.SetVision(radius);
        if(!noVision)//如果存在视野
        {
            _lantern.StartCountdown();
            _panicCom.StopPanic();
        }
    }

    public void StartPanic() => _panicCom.StartPanic();
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