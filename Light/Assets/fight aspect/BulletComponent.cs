using System;
using Components;
using GameData;
using GMVC.Utls;
using UnityEngine;
using Utls;

namespace fight_aspect 
{
    public class BulletComponent : ColliderHandlerComponent
    {
        public Spell Spell;
        public float distance;
        public float currentDistance;
        public Vector3 targetPosition;
        public Vector3 currentPosition;
        public float Speed = 2f;
        public Vector3 direction;
        public float startTime;
        public float duration=2f;//生命周期
        public BulletTracking BulletTracking;
        public Transform Target;
        bool KeepActive => Target || Time.deltaTime - startTime < duration;
        public bool IsBulletInit { get; private set; }

        public void Set(IBattleUnit owner, GameObject target, 
            BulletTracking bulletTracking, 
            float lasting,
            float speed = -1)
        {
            SetTargetTag(target.tag);
            duration = lasting;
            Spell = owner.Spell;
            Target = target.transform;
            BulletTracking = bulletTracking;
            targetPosition = target.transform.position;
            currentPosition = owner.transform.position;
            transform.position = owner.transform.position;
            direction = (targetPosition - currentPosition).normalized;
            distance = (targetPosition - currentPosition).magnitude;
            currentDistance = distance;
            startTime = Time.time;
            if (speed > 0) Speed = speed;
            this.Display(true);
            IsBulletInit = true;
        }

        void ResetBullet()
        {
            StopAllCoroutines();
            Target = null;
            currentPosition = Vector3.zero;
            direction = Vector3.zero;
            distance = 0;
            currentDistance = distance;
            startTime = Time.deltaTime;
            IsBulletInit = false;
            this.Display(false);
        }
        public void UpdateBullet()
        {
            if (!KeepActive)
            {
                ResetBullet();
                return;
            }
            //if (Time.time - startTime < Spell.Delay) return;
            Action updateAction;
            switch (BulletTracking)
            {
                case BulletTracking.Track:
                    updateAction = Tracking;
                    break;
                case BulletTracking.Direct:
                    updateAction = Direct;
                    break;
                case BulletTracking.HalfTrack:
                {
                    var switchDirect = false;
                    updateAction = HalfTrackRoutine(ref switchDirect);
                    if (switchDirect) direction = (targetPosition - currentPosition).normalized;
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!Target)
            {
                ResetBullet();
                return;
            }
            updateAction?.Invoke();
            //XArg.Format(new { transform.position, tar = Target.position }).Log(this);
            return;

            Action HalfTrackRoutine(ref bool switchDirect) //半追踪
            {
                currentDistance = (targetPosition - currentPosition).magnitude;
                if (currentDistance > distance / 2)
                    return Tracking;
                if (!switchDirect) switchDirect = true;
                return Direct;
            }

            void Tracking() //追踪攻击
                => gameObject.transform.position = Vector3
                    .MoveTowards(transform.position, Target.position, Speed * Time.deltaTime);

            void Direct() //定向攻击
                => transform.position += Speed * Time.deltaTime * direction;
        }
        
        protected override void OnHandlerEnter(Collider3DHandler handler)
        {
            if (Target && handler.root != Target.gameObject) // 如果已经有目标，且不是当前目标继续等待真正的目标
                return;
            var attackUnit = handler.root.GetComponent<IBattleUnit>();
            attackUnit.BulletImpact(this);
            ResetBullet();
        }

        protected override void OnHandlerExit(Collider3DHandler handler)
        {
            
        }
    }
    public enum BulletTracking
    {
        [InspectorName("追踪")]Track,
        [InspectorName("直飞")]Direct,
        [InspectorName("半追踪")]HalfTrack
    }
}