using System.Collections;
using GMVC.Core;
using UnityEngine;
using UnityEngine.AI;

public class EnemyComponent : PlayerTrackingComponentBase
{
    [SerializeField] NavMeshAgent nav; 
    public Transform target;
    // Start is called before the first frame update
    protected override void OnStart() => Init();

    public void Init()
    {
        nav.updateRotation = false;
    }

    protected override void OnPlayerTrackingEnter(PlayerControlComponent player)
    {
        StopAllCoroutines();
        StartCoroutine(UpdateTarget(player.transform));
    }

    protected override void OnPlayerTrackingExit(PlayerControlComponent player)
    {
        //UpdateTarget(null);
    }
    //设置速度
    public void SetSpeed(float speed) => nav.speed = speed;
    IEnumerator UpdateTarget(Transform t)//获取当前目标位置
    {
        target = t;
        while (target)
        {
            yield return new WaitForSeconds(0.2f);
            target = t;
            if (target)
            {
                nav.enabled = true;
                nav.SetDestination(target.position);
            }
            else
            {
                nav.enabled = false;
            }
        }
    }
}
