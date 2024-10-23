using fight_aspect;
using GameData;
using Sirenix.OdinInspector;
using UnityEngine;

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
        public GameObject Target;
        public bool IsCdComplete=> attackComponent.IsCDComplete;
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
            if (!Target || Target != handler.root) return;
            Target = null;
        }
        void TargetSpotted(Collider3DHandler handler)
        {
            if (Target && Target == handler.root) return;
            Target = handler.root;
        }
        void CDComplete() => SetActive(true);
        [Button("开启")]public void SetActive(bool active)
        {
            innerPar.gameObject.SetActive(active);
            glow.gameObject.SetActive(active);
        }
        public void AttackTarget() => attackComponent.Attack(Target);

    }
}