using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 2D碰撞辅助器，处理碰撞并发射事件。可以自由支持触发和碰撞机制
/// </summary>
public class Collider2DHandler : MonoBehaviour
{
    public GameObject root;
    public readonly UnityEvent<Collider2D> OnTriggerEnter=new();
    public readonly UnityEvent<Collider2D> OnTriggerExit=new();
    public readonly UnityEvent<Collision2D> OnCollisionEnter=new();
    public readonly UnityEvent<Collision2D> OnCollisionExit=new();
    void OnTriggerEnter2D(Collider2D other)=> OnTriggerEnter?.Invoke(other);
    void OnTriggerExit2D(Collider2D other) => OnTriggerExit?.Invoke(other);
    void OnCollisionEnter2D(Collision2D other)=> OnCollisionEnter?.Invoke(other);
    void OnCollisionExit2D(Collision2D other)=> OnCollisionExit?.Invoke(other);
}