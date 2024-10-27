using GMVC.Utls;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace fight_aspect
{
    public class BulletManager : MonoBehaviour
    {
        public BulletComponent bulletPrefab;
        ObjectPool<BulletComponent> pool;
        public void Init()
        {
            pool = new ObjectPool<BulletComponent>(Bullet_Spawn,
                bullets.Add,
                b=>bullets.Remove(b),
                Destroy);
        }
        #region 对象池与Update管理
        private readonly List<BulletComponent> bullets = new();
        void Update()
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                if (bullet.gameObject.activeSelf)
                    bullet.UpdateBullet();
                else pool.Release(bullet);
            }
        }
        BulletComponent Bullet_Spawn()
        {
            var bullet = Instantiate(bulletPrefab,transform);
            bullet.Display(false);
            return bullet;
        }
        #endregion
        public BulletComponent Shoot(IBattleUnit owner, Transform target, float lasting)
        {
            if (owner == null || !target) return null;
            var bullet = pool.Get();
            bullet.Set(owner, target, lasting);
            return bullet;
        }
    }
}