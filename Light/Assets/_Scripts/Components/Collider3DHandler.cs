using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 3D碰撞辅助器，处理碰撞并发射事件。可以自由支持触发和碰撞机制。
/// </summary>
[RequireComponent(typeof(Collider))]public class Collider3DHandler : MonoBehaviour
{
    public GameObject root;
    public readonly UnityEvent<Collider> OnTriggerEnterEvent=new();
    public readonly UnityEvent<Collider> OnTriggerExitEvent=new();
    public readonly UnityEvent<Collision> OnCollisionEnterEvent=new();
    public readonly UnityEvent<Collision> OnCollisionExitEvent=new();
    void OnTriggerEnter(Collider other)=> OnTriggerEnterEvent?.Invoke(other);
    void OnTriggerExit(Collider other) => OnTriggerExitEvent?.Invoke(other);
    void OnCollisionEnter(Collision other)=> OnCollisionEnterEvent?.Invoke(other);
    void OnCollisionExit(Collision other)=> OnCollisionExitEvent?.Invoke(other);
}