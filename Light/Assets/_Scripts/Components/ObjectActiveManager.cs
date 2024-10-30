using System.Collections;
using System.Collections.Generic;
using Components;
using GMVC.Utls;
using UnityEngine;

public class ObjectActiveManager : MonoBehaviour
{
    public SphereCollider playerRangeCollider => Player.visionLOD;
    public PlayerControlComponent Player;

    [Tooltip("检查物件激活状态的时间间隔")]
    public float checkInterval = 0.5f;

    // 存储管理器第一层的子物件
    List<GameObject> managedObjects = new List<GameObject>();

    // 玩家激活范围的半径
    float activationRadius;

    public void StartService(PlayerControlComponent player)
    {
        Player = player;
        // 检查必要的引用是否已设置
        if (Player == null || playerRangeCollider == null)
        {
            Debug.LogError("请在 Inspector 中设置玩家的 Transform 和 Sphere Collider。");
            return;
        }

        // 获取激活范围的半径
        activationRadius = playerRangeCollider.radius * Mathf.Max(playerRangeCollider.transform.lossyScale.x,
            playerRangeCollider.transform.lossyScale.y,
            playerRangeCollider.transform.lossyScale.z);

        // 获取管理器第一层的子物件
        foreach (Transform child in transform)
        {
            var obj = child.gameObject;
            managedObjects.Add(obj);
            obj.SetActive(false); // 初始时将物件设置为未激活
        }
        this.Display(true);
        // 开始定期检查
        Co = StartCoroutine(UpdateObjectActivation());
    }

    Coroutine Co { get; set; }

    public void StopService() => StopCoroutine(Co);

    IEnumerator UpdateObjectActivation()
    {
        var batch = managedObjects.Count / 10;
        while (true)
        {
            var playerPosition = Player.transform.position;
            yield return new WaitForSeconds(checkInterval);
            for (var index = 0; index < managedObjects.Count; index++)
            {
                if (index % batch == 0) yield return null;
                var obj = managedObjects[index];
                if (obj == null) continue;
                var distance = Vector3.Distance(playerPosition, obj.transform.position);
                if (distance <= activationRadius)
                {
                    if (!obj.activeSelf)
                    {
                        obj.SetActive(true);
                    }
                }
                else
                {
                    if (obj.activeSelf)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
    }
}