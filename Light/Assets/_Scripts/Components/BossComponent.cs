using UnityEngine;

namespace Components
{
    public class BossComponent : MonoBehaviour
    {
        public bool IsDeath => Enemy == null;
        public EnemyComponent Enemy;
    }
}