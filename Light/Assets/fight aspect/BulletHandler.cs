using GameData;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace fight_aspect
{
    public class BulletHandler: MonoBehaviour
    {
        public readonly UnityEvent<Spell,Vector3> OnBulletEvent = new();

        public void BulletImpact(BulletComponent bul)
        {
            var spell = bul.Spell;
            var impactDirection = (transform.position - bul.transform.position).normalized;
            $"攻击：{XArg.Format(spell)}".Log(this);
            OnBulletEvent?.Invoke(spell, impactDirection);
        }
    }
}