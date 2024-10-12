using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Speed=0.75f;
    public Transform player;
    public Vector3 Offset;
    [SerializeField, LabelText("采用动态镜头跟踪")] public bool dynamics;
    private void Awake()
    {
        Offset = transform.position-player.position;
    }
    private void LateUpdate()
    {
        if(player!=null)
        {
            if (dynamics)
                transform.position = Vector3.Lerp(transform.position, (player.position + Offset), Speed * Time.deltaTime);
            else
                transform.position = player.position;
        }
    }
}
