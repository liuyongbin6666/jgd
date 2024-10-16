using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Enemy_Move : MonoBehaviour
{
    public Transform player;
    NavMeshAgent nav;
    public float speed=3f;
    public Transform target;
    // Start is called before the first frame update
    public void Init()
    {
        player = FindObjectOfType<PlayerControlComponent>().transform;
        nav = GetComponent<NavMeshAgent>();
        SetSpeed(3f);
        FindTarget(player);//测试用的语句

    }
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        TrackTarget(target);//测试用的语句
    }
    void SetSpeed(float s)//设置速度
    {
        speed = s;
        nav.speed = speed;
    }
    void FindTarget(Transform t)//获取当前目标位置
    {
        target = t;
    }
    void TrackTarget(Transform t)//追击目标
    {
        if(t!=null)
        {
            nav.enabled = true;
            nav.SetDestination(player.transform.position);
        }
        else
        {
            nav.enabled = false;
        }
    }
}
