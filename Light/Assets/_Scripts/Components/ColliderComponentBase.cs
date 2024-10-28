using System;
using GameData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    /// <summary>
    /// 碰撞组件基类，处理碰撞并发射事件。可以自由支持触发和碰撞机制。
    /// </summary>
    public abstract class ColliderComponentBase : GameStartInitializer
    {
        const string UnTagged = "Untagged";
        [SerializeField] Collider3DHandler _unitCollider3D;
#if UNITY_EDITOR
        static string[] GetTags() => UnityEditorInternal.InternalEditorUtility.tags;
        [SerializeField, ValueDropdown(nameof(GetTags)), LabelText("碰撞目标标签")] 
#endif
        string _targetTag;
        //[LabelText("检测root标签")] public bool checkRootTag;
        protected override void OnGameStart() { }
        bool isInit;
        protected override void OnStart()
        {
            if (isInit) throw new Exception("重复初始化");
            isInit = true;
            _unitCollider3D?.OnTriggerEnterEvent.AddListener(Collider3DEnter);
            _unitCollider3D?.OnTriggerExitEvent.AddListener(Collider3DExit);
            _unitCollider3D?.OnCollisionEnterEvent.AddListener(c => Collider3DEnter(c.collider));
            _unitCollider3D?.OnCollisionExitEvent.AddListener(c => Collider3DExit(c.collider));
            OnGameInit();
        }
        protected void SetTargetTag(string targetTag) => _targetTag = targetTag;
        protected virtual void OnGameInit() { }
        void Collider3DExit(Collider col)
        {
            if (_targetTag == UnTagged)
            {
                OnCollider3DExit(col);
                return;
            }
            if (col.gameObject.CompareTag(_targetTag)) OnCollider3DExit(col);
        }
        void Collider3DEnter(Collider col)
        {
            if (_targetTag == UnTagged)
            {
                OnCollider3DEnter(col);
                return;
            }
            if (col.gameObject.CompareTag(_targetTag)) OnCollider3DEnter(col);
        }
        protected abstract void OnCollider3DEnter(Collider col);
        protected abstract void OnCollider3DExit(Collider col);
    }
}