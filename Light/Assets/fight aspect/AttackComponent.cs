using System.Collections;
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
    /// 通用攻击组件
    /// </summary>
    public class AttackComponent : ColliderHandlerComponent
    {
        public BulletManager bulletManager;
        [SerializeField,LabelText("攻击CD")]float cd= 1f;
        [SerializeField,LabelText("请选择射击方式")] BulletTracking bulletTracking;
        [SerializeField, LabelText("子弹维持时长")] float lasting = 1f;
        [SerializeField, LabelText("跳过第一次CD")] bool skipFirstCd;
        //[SerializeField, LabelText("执行攻击")]public bool isAttack;
        float startTime;
        GameObject target;
        public readonly UnityEvent<BulletComponent> OnAttack = new();
        public bool IsEnable { get; private set; }
        IBattleUnit BattleUnit { get; set; }
        bool IsInit { get; set; }
        public void Init(IBattleUnit unit)
        {
            BattleUnit = unit;
            IsInit = true;
        }

        public void Enable(bool enable)
        {
            if (IsEnable == enable) return;
            IsEnable = enable;
            if(!IsEnable) StopCoroutine(CountingColdDown());
            else StartCoroutine(CountingColdDown());
        }
        protected override void OnHandlerEnter(Collider3DHandler handler)
        {
            if (!IsInit) return;
            if (!IsEnable) return;
            if (target) return;
            target = handler.root;
        }

        protected override void OnHandlerExit(Collider3DHandler handler)
        {
            if (!IsInit) return;
            if (!IsEnable) return;
            if (handler.root == target) target = null;
        }
        IEnumerator CountingColdDown()
        {
            var skipCd = skipFirstCd;
            while (true)
            {
                if(!skipCd)
                {
                    yield return new WaitForSeconds(cd);
                    skipCd = false;
                }
                "Cd 结束".Log(this);
                yield return new WaitUntil(() => target);
                var bullet = bulletManager.Shoot(BattleUnit, target, bulletTracking ,lasting);
                if (!bullet) continue;
                OnAttack?.Invoke(bullet);
                $"{target?.name}存在，攻击！".Log(this);
                //isAttack = false;
            }
        }
    }
}
