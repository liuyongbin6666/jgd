using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class RainingComponent : MonoBehaviour
{
    [SerializeField] Transform followTransform;

    void Start()
    {
        if (!followTransform) return;
        StartCoroutine(FollowRoutine());
        IEnumerator FollowRoutine()
        {
            yield return new WaitForSeconds(0.1f);
            FollowLocation();
        }
    }


    [Button]public void FollowLocation()
    {
        if (!followTransform) return;
        var t = transform.position;
        transform.position = followTransform.position.ChangeY(t.y);
    }
}
