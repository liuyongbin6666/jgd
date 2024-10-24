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
        [SerializeField] AttackComponent attackComponent;
        List<GameObject> targets = new();

        public IEnumerable<GameObject> Targets
        {
            get
            {
                if (targets.Any(t => !t))
                    targets = targets.Where(t => t).ToList();
                return targets;
            }
        }
        public GameObject AimTarget => targets.Where(t => t)
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
            if (targets.Count ==0 || !targets.Contains(handler.root)) return;
            targets.Remove(handler.root);
        }
        void TargetSpotted(Collider3DHandler handler)
        {
            if (targets.Contains(handler.root)) return;
            targets.Add(handler.root);
        }
        void CDComplete() => SetActive(true);
        [Button("开启")]public void SetActive(bool active)
        {
            innerPar.gameObject.SetActive(active);
            glow.gameObject.SetActive(active);
        }
        public void AttackTarget()
        {
            if (!AimTarget) return;
            attackComponent.Attack(AimTarget);
            SetActive(false);
        }
        public void ResetCd() => attackComponent.RestartCD();
    }
}