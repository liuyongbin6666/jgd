using System;
using System.Collections;
using fight_aspect;
using GameData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Utls;

namespace Components
{
    public class EnemyComponent : PlayerTrackingComponentBase,IBattleUnit
    {
        enum TargetReaction
        {
            [InspectorName("无")]None,
            [InspectorName("追逐")]Chasing,
            [InspectorName("攻击")]Attacking,
        }
        [SerializeField] NavMeshAgent nav;
        [SerializeField] AttackComponent attackComponent;
        [LabelText("血量")]public int HP = 10;
        public Transform target;
        [LabelText("强制不移动")]public bool StopMove;//用于强制停止移动，但非状态控制
        [LabelText("法术")]public Spell spell;
        public Spell Spell => spell;
        [SerializeField,LabelText("行动")] TargetReaction reaction;
        bool IsInit { get; set; }
        protected override void OnGameInit() => Init();
        public void Init()
        {
            if(IsInit) return;
            IsInit = true;
            attackComponent.Init(this);
            nav.updateRotation = false;
            nav.enabled = true;
        }

        protected override void OnPlayerTrackingEnter(PlayerControlComponent player)
        {
            if(!IsInit)return;
            StopAllCoroutines();
            target = player.transform;
        }

        protected override void OnPlayerTrackingExit(PlayerControlComponent player)
        {
            //UpdateTarget(null);
        }
        //设置速度
        public void SetSpeed(float speed) => nav.speed = speed;

        void Update()
        {
            if (!target) reaction = TargetReaction.None;
            switch (reaction)
            {
                case TargetReaction.Chasing:
                    MoveUpdate();
                    break;
                case TargetReaction.Attacking:
                    AttackUpdate();
                    break;
            }
        }
        void AttackUpdate()
        {
            if (attackComponent.IsCDComplete)
            {
                attackComponent.Attack(target.gameObject);
                reaction = TargetReaction.Chasing;
            }else if (attackComponent.IsInRange(target))
            {
                reaction = TargetReaction.Chasing;
            }
        }
        void MoveUpdate()
        {
            var isInRange = attackComponent.IsInRange(target);
            nav.isStopped = isInRange;
            reaction = isInRange switch
            {
                true => TargetReaction.Attacking,
                _ => TargetReaction.Chasing
            };
            if (reaction != TargetReaction.Chasing) return;
            if (StopMove) nav.SetDestination(target.position);
        }

        public void BulletImpact(BulletComponent bullet)
        {
            HP -= bullet.Spell.Damage;
            if (HP <= 0) Destroy(gameObject);
        }
    }
}
