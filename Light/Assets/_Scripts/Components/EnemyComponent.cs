using fight_aspect;
using GameData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Components
{
    public class EnemyComponent : PlayerTrackingComponentBase, IBattleUnit
    {
        [SerializeField] public NavMeshAgent nav;
        [SerializeField] public AttackComponent attackComponent;
        [SerializeField] public VisionActiveComponent VisionActive;
        [SerializeField] public Rigidbody rb3D;
        [SerializeField] Animator anim; // 添加 Animator 组件
        [SerializeField, LabelText("血量")] public int HP = 10;
        public IBattleUnit Target { get; private set; }
        [LabelText("强制不移动")] public bool StopMove; // 用于强制停止移动
        [LabelText("法术")] public Spell spell;

        IGameUnitState currentState;
        bool isInitialized;

        protected override void OnGameInit() => Init();

        public void Init()
        {
            if (isInitialized) return;
            isInitialized = true;
            attackComponent.Init(this);
            //VisionActive.OnActiveEvent.AddListener(OnPlayerTriggerActive);
            nav.updateRotation = false;
            nav.enabled = true;
            // 设置初始状态为 EnemyIdleState
            SwitchState(new EnemyIdleState(this));
            ResetCD();
        }
        //void OnPlayerTriggerActive()//当玩家视野触发激活时
        //{ 
        //}
        public void SwitchState(IGameUnitState newState)
        {
            currentState?.ExitState();
            currentState = newState;
            currentState?.EnterState();
        }

        void Update()
        {
            currentState?.UpdateState();
        }

        public void UpdateAnimation(string animName)
        {
            // 播放指定名称的动画
            anim.Play(animName);
        }
        protected override void OnPlayerTrackingEnter(PlayerControlComponent player)
        {
            if (!isInitialized) return;
            Target = player;
            // 如果当前不是攻击或追逐状态，切换到追逐状态
            if (!(currentState is EnemyAttackState) && !(currentState is EnemyChaseState))
            {
                SwitchState(new EnemyChaseState(this));
            }
        }

        protected override void OnPlayerTrackingExit(PlayerControlComponent player)
        {
            //if (target == player.transform)
            //{
            //    target = null;
            //    SwitchState(new EnemyIdleState(this));
            //}
        }

        // 设置速度
        public void SetSpeed(float speed) => nav.speed = speed;

        public bool IsDeath => HP <= 0;

        public void BulletImpact(BulletComponent bullet)
        {
            var sp = bullet.Spell;
            var direction = bullet.ImpactDirection(transform);
            rb3D.AddForce(direction * sp.force, ForceMode.Impulse);
            HP -= bullet.Spell.Damage;
            if (HP <= 0)
            {
                SwitchState(new EnemyDeadState(this));
            }
        }

        public Spell CastSpell() => spell;
        public void ResetCD() => attackComponent.RestartCD();
    }
}
