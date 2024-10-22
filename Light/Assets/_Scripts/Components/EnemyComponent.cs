using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Components
{
    public class EnemyComponent : PlayerTrackingComponentBase
    {
        [SerializeField] NavMeshAgent nav; 
        public Transform target;
        public bool StopMove;
        // Start is called before the first frame update
        protected override void OnGameInit() => Init();

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
                nav.enabled = target;
                if (target && !StopMove)
                {
                    nav.SetDestination(target.position);
                }
            }
        }
    }
}
