using System;
using System.Collections.Generic;
using fight_aspect;
using GameData;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public interface IGameUnitState
    {
        enum Anims
        {
            Idle = -1,
            Move = 0,
            Attack = 1,
            React = 2,
            Death = -2,
        }
        void EnterState();
        void UpdateState();
        void ExitState();
    }

    public class PlayerControlComponent : MonoBehaviour, IBattleUnit
    {
        [SerializeField] GameLaunch gameLaunch;
        //[SerializeField] BulletHandler bulletHandler;
        [SerializeField] Rigidbody rb3D;
        public Animator anim;
        [SerializeField] Collider3DHandler bodyCollider;
        [SerializeField] LanternComponent _lantern;
        [SerializeField] SpriteRenderer renderer;
        [SerializeField] PanicComponent _panicCom;
        [SerializeField] MagicStaffComponent magicStaff;
        [SerializeField, LabelText("移动速度")] float moveSpeed = 5f;
        [SerializeField, LabelText("虫灯最大值")] int _maxLantern = 5;
        [SerializeField, LabelText("虫灯最小值")] int _minLantern = 1;
        [LabelText("法术")] public Spell Spell;
        [LabelText("玩家面朝右")]public bool faceRight;
        float MovingSpeed => moveSpeed * _movingRatio;
        [LabelText("移动摇杆")]
        public Vector2 axisMovement;

        public bool IsMoving => axisMovement != Vector2.zero || stopMoving;
        public IEnumerable<GameObject> Targets => magicStaff.Targets;
        public bool IsCdDone => magicStaff.IsCdComplete;

        [HideInInspector]
        public bool stopMoving;

        IGameUnitState currentState;

        public readonly UnityEvent OnLanternPulse = new();
        public readonly UnityEvent OnPanicFinalize = new();
        public readonly UnityEvent<int,int> OnPanicPulse = new();
        public readonly UnityEvent<GameItemBase> OnGameItemTrigger = new();
        public readonly UnityEvent<Spell> OnSpellImpact = new();
        public event Func<Spell> OnCastSpell;
        float _movingRatio = 1f;

        public void Init()
        {
            this.Display(true);
            _lantern.Init();
            _lantern.OnCountdownComplete.AddListener(OnLanternPulse.Invoke);
            _panicCom.OnPulseTrigger.AddListener(OnPanicPulse.Invoke);
            _panicCom.OnPulseComplete.AddListener(OnPanicFinalize.Invoke);
            magicStaff.Init(this);

            // 初始化状态为 Idle
            SwitchState(new PlayerIdleState(this));
        }

        void Update()
        {
            currentState?.UpdateState();
        }

        public void SwitchState(IGameUnitState newState)
        {
            currentState?.ExitState();
            currentState = newState;
            currentState?.EnterState();
        }

        void SpellImpact(Spell spell, Vector3 direction)
        {
            rb3D.AddForce(direction * spell.force, ForceMode.Impulse);
            SwitchState(new PlayerReactState(this));
            OnSpellImpact.Invoke(spell);
        }
        public void SetSpeed(float speed) => moveSpeed = speed;
        public void StopPanic() => StopAllCoroutines(); // 暂时这样停止，实际上会停止所有协程。
        public void Lantern_Update(int lantern)
        {
            var minVision = lantern > _minLantern;//虫灯最小值
            var lanternLevel = Mathf.Clamp(lantern, 0, _maxLantern);
            _movingRatio = _lantern.SetVisionLevel(lanternLevel);
            if (!minVision) return; // 视野已经最小了
            _lantern.Restart();
            _panicCom.StopIfPanic();
        }

        public void StartPanic() => _panicCom.StartPanic();

        public void UpdateAnim(IGameUnitState.Anims value)
        {
            var animInt = (int)value;
            if(value != IGameUnitState.Anims.React)//硬直需要强行播放
                if (anim.GetInteger(GameTag.AnimInt) == animInt)
                    return;
            anim.SetInteger(GameTag.AnimInt, animInt);
            anim.SetTrigger(GameTag.AnimTrigger);
        }

        public void HandleMovement()
        {
            if (axisMovement.x != 0) SetFlip(axisMovement.x);
            rb3D.MovePosition(rb3D.position + new Vector3(axisMovement.x, 0, axisMovement.y)
                * MovingSpeed * Time.deltaTime);
        }
        //设置玩家朝向
        void SetFlip(float axisX) => renderer.flipX = faceRight ? axisX < 0 : axisX > 0;

        public void SetInjured(bool isEmit)
        {
            var mpb = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_INJURED", isEmit ? 1f : 0f);
            renderer.SetPropertyBlock(mpb);
        }
        public void BulletImpact(BulletComponent bullet) =>
            SpellImpact(bullet.Spell, bullet.ImpactDirection(transform));

        public Spell CastSpell() => OnCastSpell.Invoke();
        public void GameItemInteraction(GameItemBase gameItem) => OnGameItemTrigger.Invoke(gameItem);
        public void TryAttackTarget()
        {
            if (!magicStaff.AimTarget) return;
            SetFlip(magicStaff.AimTarget.transform.position.x - transform.position.x);
            magicStaff.AttackTarget();
        }
        public void ResetAttackCD() => magicStaff.ResetCd();
        public void Die() => SwitchState(new PlayerDeathState(this));
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