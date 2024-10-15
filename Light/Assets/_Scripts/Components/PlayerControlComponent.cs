using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
public enum GameRender
{
    [InspectorName("2D")]Render_2D,
    [InspectorName("3D")]Render_3D
}
public class PlayerControlComponent : MonoBehaviour
{

    [SerializeField] GameLaunch gameLaunch;
    [SerializeField, LabelText("移动速度")] float moveSpeed = 5f;
    GameRender Mode => gameLaunch?.RenderMode?? GameRender.Render_2D;
    public bool IsMoving => axisMovement != Vector2.zero;
    [SerializeField, HideIf(nameof(Mode),GameRender.Render_3D)] Rigidbody2D rb;
    [SerializeField, HideIf(nameof(Mode), GameRender.Render_2D)] Rigidbody rb3D;
    //[SerializeField] LightVisionComponent _lightVision;
    [SerializeField, HideIf(nameof(Mode), GameRender.Render_3D)] Collider2DHandler _unitCollider;
    [SerializeField, HideIf(nameof(Mode), GameRender.Render_2D)] Collider3DHandler _unitCollider3D;
    [SerializeField] LanternComponent _lantern;
    [SerializeField] int _maxLantern = 5;
    int currentLantern = 0;
    [SerializeField] PanicComponent _panicCom;
    [SerializeField,LabelText("灯光步进")] float _lightOuterStep = 0.5f;
    [SerializeField,LabelText("周围检测层")] LayerMask _detectLayer;
    [LabelText("移动摇杆")]public Vector2 axisMovement;
    public readonly UnityEvent OnLanternTimeout = new();
    public readonly UnityEvent OnPanicFinalize = new();
    public readonly UnityEvent<int> OnPanicPulse = new();
    public readonly UnityEvent<GameItemBase> OnGameItemTrigger= new();
    public void Init(float lightOuterStep)
    {
        _lightOuterStep = lightOuterStep;
        _lantern.Init();
        _lantern.OnCountdownComplete.AddListener(()=>OnLanternTimeout?.Invoke());
        _panicCom.Init();
        _panicCom.OnPulseTrigger.AddListener(ScaryPulse);
        //_unitCollider?.OnTriggerEnter.AddListener(ColliderEnter);
        //_unitCollider?.OnCollisionEnter.AddListener(c => ColliderEnter(c.collider));
        //_unitCollider3D?.OnTriggerEnterEvent.AddListener(ColliderEnter);
        //_unitCollider3D?.OnCollisionEnterEvent.AddListener(c => ColliderEnter(c.collider));
    }

    void ColliderEnter(Collider2D col2D)
    {
        //if (CheckIf(GameTag.Firefly, FireFlyCollect)) return;
        //if (CheckIf(GameTag.GameItem, GameItemTrigger)) return;
        return;

        bool CheckIf(string objTag, UnityAction<Collider2D> onTriggerAction)
        {
            if (!col2D.CompareTag(objTag)) return false;
            onTriggerAction(col2D);
            return true;
        }
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
        this.Log($"发现：{string.Join(',',colliders.Select(c=>c.name))}");
    }
    public void SetSpeed(float speed) => moveSpeed = speed;
    public void StopPanic() => StopAllCoroutines();//暂时这样停止，实际上会停止所有协程。
    public void Lantern(int lantern)
    {
        var hasVision = lantern > 0;
        // 视野/灯笼范围
        //var radius = (lantern + 1) * _lightOuterStep;
        //_lantern.SetVision(radius);
        var lanternLevel = Mathf.Clamp(lantern, 0, _maxLantern);
        _lantern.SetVisionLevel(lanternLevel);
        if (!hasVision) return; 
        //如果存在视野
        _lantern.StartCountdown();
        _panicCom.StopIfPanic();
    }

    public void StartPanic() => _panicCom.StartPanic();
    //void Update()
    //{
    //    axisMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //}
    void FixedUpdate()
    {
        // 使用 MovePosition 进行物理移动，这样可以确保碰撞检测正常
        if(axisMovement!=Vector2.zero)
        {
            switch (Mode)
            {
                case GameRender.Render_2D:
                    rb.MovePosition(rb.position + axisMovement * moveSpeed * Time.fixedDeltaTime);
                    break;
                case GameRender.Render_3D:
                    var local = rb3D.transform.localScale;
                    var flipX = axisMovement.x < 0 ? 1 : -1;
                    rb3D.transform.localScale = new Vector3(flipX , local.y, local.z);
                    rb3D.MovePosition(rb3D.position + new Vector3(
                        axisMovement.x, 0, axisMovement.y) * moveSpeed * Time.fixedDeltaTime);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        detectionTimer += Time.fixedDeltaTime;
        if (detectionTimer >= detectionInterval)
        {
            detectionTimer = 0f;
            DetectEnemies();
        }
    }
    //为了优化FixedUpdate调用大量的2d物理来检测周围物件，原本1秒调用50次(fixedUpdate)改成1秒10次以减少gc带来的性能开销。
    float detectionInterval = 0.1f; // 检测间隔
    float detectionTimer = 0f;
    void DetectEnemies()
    {
        var colliders = _lantern.CheckForEnemiesInView(_detectLayer);
        if(colliders.Any())OnColliderOnView(colliders);
    }

    public void GameItemInteraction(GameItemBase gameItem) => OnGameItemTrigger.Invoke(gameItem);
}

internal static class PlayerControlComponentExtensions
{
    public static PlayerControlComponent GetControlFromColliderHandler(this Collider2D co)
    {
        var handler = co.GetComponent<Collider2DHandler>();
        return handler ? handler.root.GetComponent<PlayerControlComponent>() : co.GetComponent<PlayerControlComponent>();
    }    
    public static PlayerControlComponent GetControlFromColliderHandler(this Collider co)
    {
        var handler = co.GetComponent<Collider3DHandler>();
        return handler ? handler.root.GetComponent<PlayerControlComponent>() : co.GetComponent<PlayerControlComponent>();
    }
}