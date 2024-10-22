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
            [InspectorName("追逐")]Chase,
            [InspectorName("攻击")]Attack,
        }
        [SerializeField] NavMeshAgent nav;
        [SerializeField] AttackComponent attackComponent;
        [LabelText("血量")]public int HP = 10;
        public Transform target;
        public bool StopMove;
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
            attackComponent.OnAttack.AddListener(Attack);
            nav.updateRotation = false;
            nav.enabled = true;
        }

        void Attack(BulletComponent bul)
        {
            StopCoroutine(AttackWaiting());
            StartCoroutine(AttackWaiting());
            return;

            IEnumerator AttackWaiting()
            {
                StopMove = true;
                yield return new WaitWhile(() => bul.gameObject.activeSelf);
                StopMove = false;
                reaction = target == null ? TargetReaction.None : TargetReaction.Chase;
            }
        }

        protected override void OnPlayerTrackingEnter(PlayerControlComponent player)
        {
            if(!IsInit)return;
            StopAllCoroutines();
            target = player.transform;
            StartCoroutine(UpdateTarget());
        }

        protected override void OnPlayerTrackingExit(PlayerControlComponent player)
        {
            //UpdateTarget(null);
        }
        //设置速度
        public void SetSpeed(float speed) => nav.speed = speed;
        IEnumerator UpdateTarget()//获取当前目标位置
        {
            while (target)
            {
                switch (reaction)
                {
                    case TargetReaction.Chase:
                        if (target && !StopMove)
                        {
                            nav.SetDestination(target.position);
                            var distance = Vector2.Distance(transform.position.ToXZ(), target.position.ToXZ());
                            if(distance < 1.5f)
                            {
                                nav.isStopped = true;
                                reaction = TargetReaction.Attack;
                            }
                        }
                        break;
                    case TargetReaction.Attack:
                        attackComponent.Enable(target);
                        break;
                    case TargetReaction.None:
                        reaction = TargetReaction.Chase;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                yield return new WaitForSeconds(0.2f);
            }
            reaction = TargetReaction.None;
        }

        public void BulletImpact(BulletComponent bullet)
        {
            HP -= bullet.Spell.Damage;
            if (HP <= 0) Destroy(gameObject);
        }
    }
}
