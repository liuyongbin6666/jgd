using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    /// <summary>
    /// 3D碰撞辅助器，处理碰撞并发射事件。可以自由支持触发和碰撞机制。
    /// </summary>
    [RequireComponent(typeof(Collider))]public class Collider3DHandler : MonoBehaviour
    {
        public GameObject root;
        public readonly UnityEvent<Collider> OnTriggerEnterEvent=new();
        public readonly UnityEvent<Collider> OnTriggerExitEvent=new();
        public readonly UnityEvent<Collider> OnTriggerStayEvent=new();
        public readonly UnityEvent<Collision> OnCollisionEnterEvent=new();
        public readonly UnityEvent<Collision> OnCollisionExitEvent=new();
        public readonly UnityEvent<Collision> OnCollisionStayEvent=new();
        void OnTriggerEnter(Collider other)=> OnTriggerEnterEvent?.Invoke(other);
        void OnTriggerExit(Collider other) => OnTriggerExitEvent?.Invoke(other);
        void OnTriggerStay(Collider other) => OnTriggerStayEvent?.Invoke(other);
        void OnCollisionEnter(Collision other)=> OnCollisionEnterEvent?.Invoke(other);
        void OnCollisionExit(Collision other)=> OnCollisionExitEvent?.Invoke(other);
        void OnCollisionStay(Collision other) => OnCollisionStayEvent?.Invoke(other);
        public void RegEnter(UnityAction<GameObject> callback)
        {
            OnTriggerEnterEvent.AddListener(c=>callback(c.gameObject));
            OnCollisionEnterEvent.AddListener(c=>callback(c.gameObject));
        }
        public void RegExit(UnityAction<GameObject> callback)
        {
            OnTriggerExitEvent.AddListener(c => callback(c.gameObject));
            OnCollisionExitEvent.AddListener(c => callback(c.gameObject));
        }
        public void RegStay(UnityAction<GameObject> callback)
        {
            OnTriggerStayEvent.AddListener(c => callback(c.gameObject));
            OnCollisionStayEvent.AddListener(c => callback(c.gameObject));
        }
    }
}