using GameData;
using UnityEngine;
using UnityEngine.Events;

namespace fight_aspect
{
    public class BulletHandler: MonoBehaviour
    {
        public readonly UnityEvent<Spell> OnBulletEvent = new();

        public void BulletImpact(Spell spell)
        {
            //XArg.Format(spell).Log(this);
            OnBulletEvent?.Invoke(spell);
        }
    }
}