using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Config;
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
        [LabelText("技能配置")]public SpellSo SpellSo;
        [SerializeField,LabelText("子弹")] Bullet[] bullets;
        public Collider3DHandler RangeAttackCollider;
        public Spell Spell { get; private set; }
        float distance;
        float currentDistance;
        Vector3 targetPosition;
        Vector3 currentPosition;
        float Speed = 2f;
        Vector3 direction;
        float startTime;
        float duration=2f;//生命周期
        BulletTracking BulletTracking;
        [ReadOnly]public Transform Target;
        Bullet bulletCache;
        readonly List<IBattleUnit> _rangeTargets = new();
        bool KeepActive => Target || Time.deltaTime - startTime < duration;
        public bool IsBulletInit { get; private set; }
        public Vector3 ImpactDirection(Transform body) => (body.position - transform.position).normalized;
        IEnumerable<string> GetSpellNames() => SpellSo.Spells.Select(s=>s.SpellName);
        IBattleUnit Caster { get; set; }
        protected override void OnGameInit()
        {
            RangeAttackCollider.OnTriggerEnterEvent.AddListener(OnRangeRegister);
            base.OnGameInit();
        }

        public void Set(IBattleUnit owner, GameObject target,
            float lasting)
        {
            SetTargetTag(target.tag);
            duration = lasting;
            Spell = owner.CastSpell();
            Caster = owner;
            Target = target.transform;
            BulletTracking = Spell.Tracking;
            targetPosition = target.transform.position;
            currentPosition = owner.transform.position;
            transform.position = owner.transform.position;
            direction = (targetPosition - currentPosition).normalized;
            distance = (targetPosition - currentPosition).magnitude;
            currentDistance = distance;
            startTime = Time.time;
            Speed = Spell.Speed;
            ShowBullet(Spell.SpellName);
            RangeAttackCollider.Display(Spell.RangeDamage);
            this.Display(true);
            IsBulletInit = true;
        }


        void ShowBullet(string spellName)
        {
            bulletCache = bullets.FirstOrDefault(b => b.spellName == spellName);
            if(bulletCache==null)
                Debug.LogError($"{nameof(ShowBullet)}:找不到子弹类型: {spellName}");
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
            Caster = null;
            _rangeTargets.Clear();
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
            var unit = handler.root.GetComponent<IBattleUnit>();
            unit.BulletImpact(this);
            foreach (var target in _rangeTargets) 
                target.BulletImpact(this);
            StartExplosion();
        }

        void OnRangeRegister(Collider col)
        {
            var handler = col.GetComponent<Collider3DHandler>();
            if(handler==null)return;
            var unit = handler.root.GetComponent<IBattleUnit>();
            if (unit == null || unit == Caster) return;
            _rangeTargets.Add(unit);
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
            [ValueDropdown("@((BulletComponent)$property.Tree.WeakTargets[0]).GetSpellNames()")] public string spellName;
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