using System.Collections;
using System.Collections.Generic;
using GameData;
using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

namespace Components
{
    public abstract class RandomObjectSpawner<T> : MonoBehaviour where T : MonoBehaviour
    {
        [LabelText("预制体")] public T prefab;
        [SerializeField, LabelText("玩家")] protected PlayerControlComponent playerControlComponent;
        [SerializeField, LabelText("对象总数")] protected int maxObjects = 30;
        [SerializeField, LabelText("检查间隔")] protected float updateInterval = 1f;
        [SerializeField] protected Transform pool;
        [LabelText("外径")] public SphereCollider outerCollider; // 生成范围碰撞器
        [LabelText("内径")] public SphereCollider innerCollider; // 生成范围碰撞器

        protected List<T> activeObjects = new List<T>();
        protected ObjectPool<T> objectPool;
        protected Transform Player => playerControlComponent.transform;
        Coroutine Co;

        public void StartService(PlayerControlComponent player)
        {
            playerControlComponent = player;
            objectPool = new ObjectPool<T>(SpawnObject, GetObject, RecycleObject, Destroy);
            Co = StartCoroutine(UpdateObjects());
        }

        public void StopService()
        {
            StopCoroutine(Co);
            Co = null;
            // 回收所有活动的对象
            for (int i = activeObjects.Count - 1; i >= 0; i--) 
                RecycleObject(activeObjects[i]);
            activeObjects.Clear();
            // 清除对象池
            objectPool.Clear();
        }

        IEnumerator UpdateObjects()
        {
            while (true)
            {
                yield return new WaitUntil(GenerateObjects);
                yield return new WaitForSeconds(updateInterval);
            }
        }

        bool GenerateObjects()
        {
            // 回收超出范围的对象
            RecycleObjects();
            // 生成新的对象
            while (activeObjects.Count < maxObjects) objectPool.Get();
            return true;
        }

        Vector3 GetRandomPosition()
        {
            var innerRadius = innerCollider.radius * innerCollider.transform.lossyScale.x;
            var outerRadius = outerCollider.radius * outerCollider.transform.lossyScale.x;

            var angle = Random.Range(0, Mathf.PI * 2);
            var radius = Random.Range(innerRadius, outerRadius);
            var offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            var position = Player.position + offset;

            // 确保位置在导航网格上
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                return hit.position;

            // 如果位置不可行，则重新生成
            return GetRandomPosition();
        }

        void RecycleObjects()
        {
            var outerRadius = outerCollider.radius * outerCollider.transform.lossyScale.x;
            var sqrOuterRadius = outerRadius * outerRadius;

            for (var i = activeObjects.Count - 1; i >= 0; i--)
            {
                var obj = activeObjects[i];
                if (!obj)
                {
                    activeObjects.RemoveAt(i);
                    continue;
                }
                var sqrDistance = (obj.transform.position - Player.position).sqrMagnitude;
                if (sqrDistance > sqrOuterRadius)
                {
                    RecycleObject(obj);
                }
            }
        }

        protected virtual T SpawnObject()
        {
            var obj = Instantiate(prefab, pool);
            GetObject(obj);
            return obj;
        }

        void GetObject(T obj)
        {
            var spawnPosition = GetRandomPosition();
            obj.transform.position = spawnPosition;
            obj.gameObject.SetActive(true);
            activeObjects.Add(obj);
            Get(obj);
        }

        protected abstract void Get(T obj);

        void RecycleObject(T obj)
        {
            Recycle(obj);
            obj.gameObject.SetActive(false);
            activeObjects.Remove(obj);
        }

        protected abstract void Recycle(T obj);
    }
}