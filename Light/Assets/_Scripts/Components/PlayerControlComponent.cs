using System;
using System.Collections;
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
        enum Activities
        {
            Idle = -1,
            Move = 0,
            Attack = 1,
            React = 2,
        }
        [SerializeField] GameLaunch gameLaunch;
        [SerializeField] BulletHandler bulletHandler;
        [SerializeField] Rigidbody rb3D;
        [SerializeField] Animator anim;
        [SerializeField] Collider3DHandler bodyCollider;
        [SerializeField] LanternComponent _lantern;
        [SerializeField] SpriteRenderer renderer;
        [SerializeField] PanicComponent _panicCom;
        [SerializeField] MagicStaffComponent magicStaff;
        [SerializeField, LabelText("移动速度")] float moveSpeed = 5f;
        [SerializeField,LabelText("最大虫灯数")] int _maxLantern = 5;
        //[SerializeField,LabelText("周围检测层")] LayerMask _detectLayer;
        [LabelText("移动摇杆")]public Vector2 axisMovement;
        public Spell Spell => magicStaff.Spell;
        public bool IsMoving => axisMovement != Vector2.zero || stopMoving;
        Activities activity;
        bool stopMoving;
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
            magicStaff.Init(this);
            StartCoroutine(MainUpdateRoutine());
        }

        void SpellImpact(Spell spell,Vector3 direction)
        {
            rb3D.AddForce(direction* spell.force, ForceMode.Impulse);// 击退
            SwitchActivity(Activities.React);
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
        public void SetSpeed(float speed) => moveSpeed = speed;
        public void StopPanic() => StopAllCoroutines();//暂时这样停止，实际上会停止所有协程。
        public void Lantern(int lantern)
        {
            var hasVision = lantern > 0;
            var lanternLevel = Mathf.Clamp(lantern, 0, _maxLantern);
            _lantern.SetVisionLevel(lanternLevel);
            if (!hasVision) return; 
            //如果存在视野
            _lantern.StartCountdown();
            _panicCom.StopIfPanic();
        }

        public void StartPanic() => _panicCom.StartPanic();
        #region 状态
        IEnumerator MainUpdateRoutine()
        {
            while (true)
            {
                switch (activity)
                {
                    case Activities.Idle:
                        IdleUpdate();
                        break;
                    case Activities.Move:
                        MoveUpdate();
                        break;
                    case Activities.Attack:
                        //移动会打断攻击
                        if (IsMoving)
                        {
                            StopAttackCo();
                            SwitchActivity(Activities.Move);
                        }
                        break;
                    case Activities.React: break;// 协程处理
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                yield return null;
            }
        }

        void StopAttackCo()
        {
            if (attackCo == null) return;
            StopCoroutine(attackCo);
            attackCo = null;
        }
        void StopReactCo()
        {
            if (reachCo == null) return;
            StopCoroutine(reachCo);
            reachCo = null;
        }
        void ReactStart()//硬直打断所有状态
        {
            StopReactCo();
            StopAttackCo();
            activity = Activities.React;
            StartCoroutine(ReactRoutine());
            return;

            IEnumerator ReactRoutine()
            {
                SetInjured(true);
                stopMoving = true;
                yield return new WaitForSeconds(0.3f);
                SetInjured(false);
                stopMoving = false;
                SwitchActivity(Activities.Idle);
            }

            void SetInjured(bool isEmit)
            {
                var mpb = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_INJURED", isEmit ? 1f : 0f);
                renderer.SetPropertyBlock(mpb);
            }
        }

        Coroutine reachCo;
        Coroutine attackCo;
        void AttackStart()
        {
            if (activity == Activities.Attack) return;//第一次才会进入,再进就无效
            activity = Activities.Attack;
            attackCo = StartCoroutine(AttackRoutine());
        }
        IEnumerator AttackRoutine()
        {
            yield return new WaitUntil(() => magicStaff.IsCdComplete);
            yield return new WaitForSeconds(0.5f);
            "开始攻击".Log(this);
            magicStaff.AttackTarget();
            SwitchActivity(Activities.Idle);
        }

        void IdleUpdate()
        {
            if (IsMoving)
            {
                SwitchActivity(Activities.Move);
                return;
            }
            if (magicStaff.Target)
                SwitchActivity(Activities.Attack);
        }
        void MoveUpdate()
        {
            if (!IsMoving)
            {
                SwitchActivity(Activities.Idle);
                return;
            }
            if (axisMovement.x != 0) renderer.flipX = axisMovement.x > 0;
            // 使用 MovePosition 进行物理移动，这样可以确保碰撞检测正常
            rb3D.MovePosition(rb3D.position + new Vector3(
                    axisMovement.x, 0, axisMovement.y)
                * moveSpeed * Time.deltaTime);
        }
        void SwitchActivity(Activities act)
        {
            if (act == Activities.React) //硬直状态强制切换
            {
                ReactStart();
                UpdateAnim();
                return;
            }
            if (activity == Activities.React) return; //相同状态或是硬直不切换
            switch (act)
            {
                case Activities.Idle:
                case Activities.Move:
                    activity = act;
                    break; // update处理
                case Activities.Attack:
                    AttackStart();
                    break;
                case Activities.React: break;//上面处理了，这里不处理
                default:
                    throw new ArgumentOutOfRangeException(nameof(act), act, null);
            }
            UpdateAnim();
        }
        #endregion
        void UpdateAnim()
        {
            var animInt = (int)activity;
            if (anim.GetInteger(GameTag.AnimInt) == animInt) return;
            anim.SetInteger(GameTag.AnimInt, animInt);
            anim.SetTrigger(GameTag.AnimTrigger);
        }

        public void GameItemInteraction(GameItemBase gameItem) => OnGameItemTrigger.Invoke(gameItem);
        public void BulletImpact(BulletComponent bullet) => bulletHandler.BulletImpact(bullet);
    }
    /// <summary>
    /// 玩家控制组件扩展，主要是获取玩家控制组件
    /// </summary>
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