using System.Collections;
using GMVC.Utls;
using UnityEngine;
using Utls;

namespace Components
{
    public class EnemyIdleState : IGameUnitState
    {
        EnemyComponent enemy;

        public EnemyIdleState(EnemyComponent enemy)
        {
            this.enemy = enemy;
        }

        public void EnterState()
        {
            // 进入空闲状态的逻辑
            enemy.nav.isStopped = true;
            enemy.PlayAnimation(IGameUnitState.Anims.Idle);
        }

        public void UpdateState()
        {
            // 如果有目标，切换到追逐状态
            if (enemy.Target != null)
            {
                enemy.SwitchState(new EnemyChaseState(enemy));
            }
        }

        public void ExitState()
        {
            // 离开空闲状态的逻辑
        }
    }
    public class EnemyChaseState : IGameUnitState
    {
        EnemyComponent enemy;

        public EnemyChaseState(EnemyComponent enemy)
        {
            this.enemy = enemy;
        }

        public void EnterState()
        {
            // 进入追逐状态的逻辑
            enemy.nav.isStopped = false;
            enemy.PlayAnimation(IGameUnitState.Anims.Move);
        }

        public void UpdateState()
        {
            if (enemy.Target == null)
            {
                enemy.SwitchState(new EnemyIdleState(enemy));
                return;
            }

            if (enemy.StopMove)
            {
                enemy.nav.isStopped = true;
            }
            else
            {
                var flipX = enemy.Target.transform.position.x < enemy.transform.position.x;
                enemy.VisionActive.renderer.flipX = flipX;
                enemy.nav.SetDestination(enemy.Target.transform.position);
            }

            // 如果在攻击范围内，切换到攻击状态
            var inRange = enemy.attackComponent.IsInRange(enemy.Target.transform);
            if (inRange)
            {
                enemy.SwitchState(new EnemyAttackState(enemy));
            }
        }

        public void ExitState()
        {
            // 离开追逐状态的逻辑
        }
    }
    public class EnemyAttackState : IGameUnitState
    {
        EnemyComponent enemy;
        bool isAttacking;
        float attackDelay = 1f;

        public EnemyAttackState(EnemyComponent enemy)
        {
            this.enemy = enemy;
        }

        public void EnterState()
        {
            // 进入攻击状态的逻辑
            enemy.nav.isStopped = true;
            isAttacking = false;
        }

        public void UpdateState()
        {
            if (enemy.Target == null || !enemy.attackComponent.IsCooldown)
            {
                enemy.SwitchState(new EnemyIdleState(enemy));
                return;
            }
            if (!enemy.attackComponent.IsInRange(enemy.Target.transform))
            {
                enemy.SwitchState(new EnemyChaseState(enemy));
                return;
            }
            attackDelay -= Time.deltaTime;
            if (!isAttacking)
            {
                isAttacking = true;
                enemy.PlayAnimation(IGameUnitState.Anims.Attack);
                //enemy.attackComponent.Attack(enemy.Target);
            }
            if (attackDelay <= 0)
            {
                enemy.ResetCD();
            }
        }
        public void ExitState() { }
    }
    public class EnemyDeadState : IGameUnitState
    {
        EnemyComponent enemy;

        public EnemyDeadState(EnemyComponent enemy)
        {
            this.enemy = enemy;
        }

        public void EnterState()
        {
            // 进入死亡状态的逻辑
            enemy.nav.isStopped = true;
            enemy.PlayAnimation(IGameUnitState.Anims.Death);
            enemy.StartCoroutine(DestroyAfterDelay());
        }

        public void UpdateState()
        {
            // 死亡状态不需要更新
        }

        public void ExitState()
        {
            // 离开死亡状态的逻辑
        }

        IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(1f); // 等待2秒后销毁
            Object.Destroy(enemy.gameObject);
        }
    }

}