using System.Collections.Generic;
using System.Linq;
using fight_aspect;
using GameData;
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
        [LabelText("法术")]public Spell Spell;
        public List<GameObject> Targets;
        public bool IsCdComplete=> attackComponent.IsCooldown;
        public float AttackDelay => Spell.Delay;
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
            if (Targets.Count ==0 || !Targets.Contains(handler.root)) return;
            Targets.Remove(handler.root);
        }
        void TargetSpotted(Collider3DHandler handler)
        {
            if (Targets.Contains(handler.root)) return;
            Targets.Add(handler.root);
        }
        void CDComplete() => SetActive(true);
        [Button("开启")]public void SetActive(bool active)
        {
            innerPar.gameObject.SetActive(active);
            glow.gameObject.SetActive(active);
        }
        public void AttackTarget()
        {
            attackComponent.Attack(Targets.OrderBy(t =>
                Vector2.Distance(t.transform.position.ToXY(), transform.position.ToXY()))
                .FirstOrDefault());
        }
        public void ResetCd() => attackComponent.RestartCD();
    }
}