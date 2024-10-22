using System;
using System.Collections;
using System.Linq;
using fight_aspect;
using GameData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Components
{
    public class PlayerControlComponent : MonoBehaviour,IBattleUnit
    {
        enum PlayerAnim
        {
            Idle = -1,
            Move = 0,
            Attack = 1,
            React = 2,
        }
        [SerializeField] GameLaunch gameLaunch;
        [SerializeField, LabelText("移动速度")] float moveSpeed = 5f;
        [SerializeField] BulletHandler bulletHandler;
        [SerializeField] Rigidbody rb3D;
        [SerializeField] Animator anim;
        [SerializeField] Collider3DHandler _unitCollider3D;
        [SerializeField] LanternComponent _lantern;
        [SerializeField] SpriteRenderer renderer;
        [SerializeField] AttackComponent attackComponent;
        [SerializeField,LabelText("最大虫灯数")] int _maxLantern = 5;
        [SerializeField] PanicComponent _panicCom;
        [SerializeField,LabelText("周围检测层")] LayerMask _detectLayer;
        [LabelText("移动摇杆")]public Vector2 axisMovement;
        [LabelText("法术")] public Spell spell;
        public Spell Spell => spell;
        public bool IsMoving => axisMovement != Vector2.zero;
        Transform body => _unitCollider3D.transform;

        public bool stopMoving;
        public readonly UnityEvent OnLanternTimeout = new();
        public readonly UnityEvent OnPanicFinalize = new();
        public readonly UnityEvent<int> OnPanicPulse = new();
        public readonly UnityEvent<GameItemBase> OnGameItemTrigger= new();
        public readonly UnityEvent<Spell> OnSpellImpact = new();
        public void Init()
        {
            _lantern.Init();
            _lantern.OnCountdownComplete.AddListener(OnLanternTimeout.Invoke);
            _panicCom.Init();
            _panicCom.OnPulseTrigger.AddListener(ScaryPulse);
            bulletHandler.OnBulletEvent.AddListener(SpellImpact);
            attackComponent.Init(this);
            attackComponent.OnAttack.AddListener(_=>AnimSet(PlayerAnim.Attack));
            attackComponent.Enable(true);
            //_unitCollider3D?.OnTriggerEnterEvent.AddListener(ColliderEnter);
            //_unitCollider3D?.OnCollisionEnterEvent.AddListener(c => ColliderEnter(c.collider));
        }

        void SpellImpact(Spell spell,Vector3 direction)
        {
            rb3D.AddForce(direction* spell.force, ForceMode.Impulse);// 击退
            AnimSet(PlayerAnim.React);
            OnSpellImpact.Invoke(spell);
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
        void OnColliderOnView(Collider[] colliders)
        {
            $"发现：{string.Join(',',colliders.Select(c=>c.name))}".Log(this);
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
        void FixedUpdate()
        {
            // 使用 MovePosition 进行物理移动，这样可以确保碰撞检测正常
            if (axisMovement != Vector2.zero && !stopMoving)
            {
                var local = body.localScale;
                var flipX = axisMovement.x switch
                {
                    <= 0 => 1,
                    > 0 => -1,
                    _ => throw new ArgumentOutOfRangeException()
                };
                body.localScale = local.ChangeX(flipX);
                rb3D.MovePosition(rb3D.position + new Vector3(
                                      axisMovement.x, 0, axisMovement.y) 
                    * moveSpeed * Time.fixedDeltaTime);
                AnimSet(PlayerAnim.Move);
            }
            else
                AnimSet(PlayerAnim.Idle);

            detectionTimer += Time.fixedDeltaTime;
            if (detectionTimer >= detectionInterval)
            {
                detectionTimer = 0f;
                DetectEnemies();
            }
        }

        bool isReactingSpell;

        void AnimSet(PlayerAnim playerAnim)
        {
            if (isReactingSpell) return; // 对于React(强制性)状态，不再切换
            var animInt = (int)playerAnim;
            
            switch (playerAnim)
            {
                case PlayerAnim.React:
                    StartCoroutine(ReactRoutine(0.3f));
                    break;
                case PlayerAnim.Attack:
                    StartCoroutine(ReactRoutine(0.5f));
                    break;
                case PlayerAnim.Idle:
                case PlayerAnim.Move:
                default:
                    TriggerAnim();
                    break;
            }
            return;

            IEnumerator ReactRoutine(float sec)
            {
                TriggerAnim();
                SetInjured(true);
                stopMoving = true;
                isReactingSpell = true;
                yield return new WaitForSeconds(sec);
                SetInjured(false);
                isReactingSpell = false;
                stopMoving = false;
            }

            void TriggerAnim()
            {
                if (anim.GetInteger(GameTag.AnimInt) == animInt) return;
                anim.SetInteger(GameTag.AnimInt, animInt);
                anim.SetTrigger(GameTag.AnimTrigger);
            }

            void SetInjured(bool isEmit)
            {
                var mpb = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_INJURED", isEmit ? 1f : 0f);
                renderer.SetPropertyBlock(mpb);
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
        public void BulletImpact(BulletComponent bullet) => bulletHandler.BulletImpact(bullet);
    }

    internal static class PlayerControlComponentExtensions
    {
        public static PlayerControlComponent GetPlayerControlFromColliderHandler(this GameObject co)
        {
            var handler = co.GetComponent<Collider3DHandler>();
            return handler ? handler.root.GetComponent<PlayerControlComponent>() : co.GetComponent<PlayerControlComponent>();
        }    
        public static PlayerControlComponent GetPlayerControlFromColliderHandler(this Collider2D co)
        {
            var handler = co.GetComponent<Collider2DHandler>();
            return handler ? handler.root.GetComponent<PlayerControlComponent>() : co.GetComponent<PlayerControlComponent>();
        }
        public static PlayerControlComponent GetPlayerControlFromColliderHandler(this Collider co) =>
            co.gameObject.GetPlayerControlFromColliderHandler();
    }
}