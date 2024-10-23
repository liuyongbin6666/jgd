using System.Collections;
using System.Collections.Generic;
using Components;
using GameData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace fight_aspect
{
    public interface IBattleUnit
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        Spell Spell{ get; }
        void BulletImpact(BulletComponent bullet);
    }
    /// <summary>
    /// 通用攻击组件,用于处理攻击CD,攻击范围,攻击目标
    /// </summary>
    public class AttackComponent : ColliderHandlerComponent
    {
        public BulletManager bulletManager;
        [SerializeField,LabelText("攻击CD")]float cd= 1f;
        [SerializeField,LabelText("请选择射击方式")] BulletTracking bulletTracking;
        [SerializeField, LabelText("子弹维持时长")] float lasting = 1f;
        //[SerializeField, LabelText("执行攻击")]public bool isAttack;
        float startTime;
        readonly List<GameObject> targets = new();
        //public readonly UnityEvent<BulletComponent> OnAttack = new();
        public readonly UnityEvent<Collider3DHandler> OnTargetSpotted = new();
        public readonly UnityEvent<Collider3DHandler> OnTargetLeave= new();
        public readonly UnityEvent OnCdComplete = new();
        IBattleUnit BattleUnit { get; set; }
        bool IsInit { get; set; }
        public bool IsCooldown { get; private set; }
        public void Init(IBattleUnit unit)
        {
            BattleUnit = unit;
            IsInit = true;
        }
        public bool IsInRange(Transform tran) => targets.Contains(tran.gameObject);
        /// <summary>
        /// 重置cd
        /// </summary>
        public void RestartCD()
        {
            IsCooldown = false;
            if(CoundownCo!=null)
            {
                StopCoroutine(CoundownCo);
                CoundownCo = null;
            }
            CoundownCo = StartCoroutine(CountingCooldown());
        }

        Coroutine CoundownCo { get; set; }

        //当发现目标通知上层
        protected override void OnHandlerEnter(Collider3DHandler handler)
        {
            if (!IsInit) return;
            if (targets.Contains(handler.root)) return;
            targets.Add(handler.root);
            OnTargetSpotted.Invoke(handler);
        }
        //当目标离开通知上层
        protected override void OnHandlerExit(Collider3DHandler handler)
        {
            if (!IsInit) return;
            if (!targets.Contains(handler.root)) return;
            targets.Remove(handler.root);
            OnTargetLeave.Invoke(handler);
        }
        public bool Attack(GameObject target)
        {
            var canAttack = target && IsCooldown;
            if(!canAttack) return false;
            var bullet = bulletManager.Shoot(BattleUnit, target, bulletTracking, lasting);
            if (bullet) RestartCD();
            return bullet;
        }
        IEnumerator CountingCooldown()
        {
            yield return new WaitForSeconds(cd);
            IsCooldown = true;
            OnCdComplete.Invoke();
            CoundownCo = null;
        }
    }
}
