using System;
using System.Linq;
using Components;
using DG.Tweening;
using GameData;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace fight_aspect 
{
    public class BulletComponent : ColliderHandlerComponent
    {
        [SerializeField,LabelText("子弹")] Bullet[] bullets;
        public Spell Spell { get; private set; }
        float distance;
        float currentDistance;
        Vector3 targetPosition;
        Vector3 currentPosition;
        public float Speed = 2f;
        Vector3 direction;
        float startTime;
        float duration=2f;//生命周期
        BulletTracking BulletTracking;
        public Transform Target;
        Bullet bulletCache;
        bool KeepActive => Target || Time.deltaTime - startTime < duration;
        public bool IsBulletInit { get; private set; }
        public Vector3 ImpactDirection(Transform body) => (body.position - transform.position).normalized;
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
            ShowBullet(Spell.Type);
            this.Display(true);
            IsBulletInit = true;
        }

        void ShowBullet(Spell.Types type)
        {
            bulletCache = bullets.FirstOrDefault(b => b.spellType == type);
            if(bulletCache==null)
                Debug.LogError($"{nameof(ShowBullet)}:找不到子弹类型: {type}");
            bulletCache.ShowBullet(true);
        }

        void ResetBullet()
        {
            StopAllCoroutines();
            foreach (var bullet in bullets) bullet.Reset();
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
            StartExplosion();
        }

        void StartExplosion()
        {
            DOTween.Sequence().AppendCallback(() =>
                {
                    bulletCache.ShowBullet(false);
                    bulletCache.ShowExplode(true);
                })
                .AppendInterval(0.5f)
                .AppendCallback(ResetBullet);
        }

        protected override void OnHandlerExit(Collider3DHandler handler)
        {
            
        }

        [Serializable]class Bullet
        {
            [LabelText("魔法类型")]public Spell.Types spellType;
            [LabelText("子弹"),SerializeField] GameObject bullet;
            [LabelText("爆炸"),SerializeField] GameObject explode;

            public void Reset()
            {
                bullet.Display(false);
                explode.Display(false);
            }
            public void ShowBullet(bool display) => bullet.Display(display);
            public void ShowExplode(bool display) => explode.Display(display);
        }
    }
    public enum BulletTracking
    {
        [InspectorName("追踪")]Track,
        [InspectorName("直飞")]Direct,
        [InspectorName("半追踪")]HalfTrack
    }
}