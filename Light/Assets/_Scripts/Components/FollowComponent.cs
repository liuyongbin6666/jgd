using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// 跟踪控件，用于跟踪目标位置
/// </summary>
public class FollowComponent : GameStartInitializer
{
    [SerializeField] Transform followTransform;
    [SerializeField] float delaySec = 0.1f;

    protected override void OnGameStart() => Init();
    void Init()
    {
        if (!followTransform) return;
        StartCoroutine(FollowRoutine());
        IEnumerator FollowRoutine()
        {
            while (followTransform)
            {
                yield return new WaitForSeconds(delaySec);
                FollowLocation();
            }
        }
    }
    [Button("跟踪物件位置")]public void FollowLocation()
    {
        if (!followTransform) return;
        var t = transform.position;
        transform.position = followTransform.position.ChangeY(t.y);
    }
}
