using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine.Pool;

public class FireflySpawner : GameStartInitializer
{
    public GameObject fireflyPrefab;
    // 将 innerRadius 和 outerRadius 改为 Collider
    [SerializeField,LabelText("玩家")]PlayerControlComponent playerControlComponent;
    [SerializeField,LabelText("萤火虫总数")]int maxFireflies = 50;
    [SerializeField,LabelText("检查间隔")]float updateInterval = 1f;
    [SerializeField] Transform pool;
    [LabelText("外径")]public SphereCollider outerCollider; // 萤火虫生成范围碰撞器
    [LabelText("内径")]public SphereCollider innerCollider; // 萤火虫生成范围碰撞器
    List<GameObject> activeFireflies = new List<GameObject>();

    ObjectPool<GameObject> fireflyPool;
    Transform Player=> playerControlComponent.transform;

    public override void Initialization()
    {
        fireflyPool = new ObjectPool<GameObject>(Firefly_Spawn, OnGet, Firefly_Recycle, Destroy);
        StartCoroutine(UpdateFireflies());
    }

    IEnumerator UpdateFireflies()
    {
        while (true)
        {
            yield return new WaitUntil(GenerateFireflies);
            yield return new WaitForSeconds(updateInterval);
        }
    }

    bool GenerateFireflies()
    {
        // 回收萤火虫
        RecycleFireflies();
        // 生成萤火虫
        while (activeFireflies.Count < maxFireflies) fireflyPool.Get();
        return true;
    }

    Vector3 GetRandomPosition()
    {
        var innerRadius = innerCollider.radius * innerCollider.transform.lossyScale.x;
        var outerRadius = outerCollider.radius * outerCollider.transform.lossyScale.x;

        var angle = Random.Range(0, Mathf.PI * 2);// 随机生成 0 到 2PI 的角度
        var radius = Random.Range(innerRadius, outerRadius); // 在内外半径之间随机生成半径
        var offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;// 计算偏移量
        var position = Player.position + offset;// 偏移位置

        // 确保位置在导航网格上
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 2f, NavMesh.AllAreas))
            return hit.position;

        // 如果位置不可行，则重新生成
        return GetRandomPosition();
    }

    void RecycleFireflies()
    {
        var outerRadius = outerCollider.radius * outerCollider.transform.lossyScale.x;
        var sqrOuterRadius = outerRadius * outerRadius;

        for (var i = activeFireflies.Count - 1; i >= 0; i--)
        {
            var firefly = activeFireflies[i];
            if (!firefly)
            {
                activeFireflies.RemoveAt(i);
                continue;
            }
            var sqrDistance = (firefly.transform.position - Player.position).sqrMagnitude;
            if (!(sqrDistance > sqrOuterRadius)) continue;
            Firefly_Recycle(firefly);
        }
    }
    GameObject Firefly_Spawn()
    {
        var firefly = Instantiate(fireflyPrefab, pool);
        OnGet(firefly);
        return firefly;
    }
    void OnGet(GameObject firefly)
    {
        var spawnPosition = GetRandomPosition();
        firefly.transform.position = spawnPosition;
        firefly.Display(true);
        activeFireflies.Add(firefly);
    }

    void Firefly_Recycle(GameObject firefly)
    {
        firefly.Display(false);
        activeFireflies.Remove(firefly);
    }
}