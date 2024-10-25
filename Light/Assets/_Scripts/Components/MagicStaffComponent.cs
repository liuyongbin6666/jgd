using System.Collections.Generic;
using System.Linq;
using fight_aspect;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace Components
{
    /// <summary>
    /// 魔法杖，处理目标检测和攻击CD
    /// </summary>
    public class MagicStaffComponent : MonoBehaviour
    {
        [SerializeField,LabelText("内光")] ParticleSystem innerPar;
        [SerializeField,LabelText("散光")] ParticleSystem glow;
        public AttackComponent attackComponent;
        List<IBattleUnit> targets = new();

        public IEnumerable<IBattleUnit> Targets
        {
            get
            {
                if (targets.Any(t=> !t.gameObject || !IsAvailable(t)))//必须检查gamgObject，否则会报错
                    targets = targets.Where(IsAvailable).ToList();
                return targets;
            }
        }

        static bool IsAvailable(IBattleUnit t) => t.gameObject && !t.IsDeath;

        public IBattleUnit AimTarget => targets.Where(IsAvailable)
            .OrderBy(t => Vector2.Distance(t.transform.position.ToXY(), transform.position.ToXY()))
            .FirstOrDefault();
        public bool IsCdComplete=> attackComponent.IsCooldown;
        public void Init(IBattleUnit unit)
        {
            attackComponent.Init(unit);
            attackComponent.OnCdComplete.AddListener(CDComplete);
            attackComponent.OnTargetSpotted.AddListener(TargetSpotted);
            attackComponent.OnTargetLeave.AddListener(TargetLeave);
            attackComponent.RestartCD();
        }
        void TargetLeave(Collider3DHandler handler)
        {
            var battleUnit = handler.root.GetComponent<IBattleUnit>();
            if (battleUnit == null) return;
            if (targets.Count ==0 || !targets.Contains(battleUnit)) return;
            targets.Remove(battleUnit);
        }
        void TargetSpotted(Collider3DHandler handler)
        {
            var battleUnit = handler.root.GetComponent<IBattleUnit>();
            if (battleUnit == null) return;
            if (targets.Contains(battleUnit)) return;
            targets.Add(battleUnit);
        }
        void CDComplete() => SetActive(true);
        [Button("开启")]public void SetActive(bool active)
        {
            innerPar.gameObject.SetActive(active);
            glow.gameObject.SetActive(active);
        }
        public void AttackTarget()
        {
            if (AimTarget == null) return;
            attackComponent.Attack(AimTarget);
            SetActive(false);
        }
        public void ResetCd() => attackComponent.RestartCD();
    }
}