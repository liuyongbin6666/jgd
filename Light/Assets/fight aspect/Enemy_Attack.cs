using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Enemy_Attack : MonoBehaviour
{
    public Rigidbody rig;
    public NavMeshAgent nav;
    public Transform player;
    public Transform target;
    public void Init()
    {
        nav.updateRotation = false;
    }
    private void Start()
    {
        Init();
    }
    private void OnTriggerEnter(Collider other)//玩家进入攻击范围
    {

        if (other.tag == "Player")
        {           
            StartCoroutine(UpdateTarget(player.transform));
        }
    }
    private void OnTriggerExit(Collider other)//玩家脱离攻击范围
    {
        if(other.tag=="Player")
        {
            StopAllCoroutines();
        }
    }
    private void OnCollisionEnter(Collision collision)//攻击玩家
    {
        Debug.Log("攻击");
    }

    //设置速度
    public void SetSpeed(float speed) => nav.speed = speed;
    IEnumerator UpdateTarget(Transform t)//获取当前目标位置
    {
        target = t;
        while (target)
        {
            yield return new WaitForSeconds(0.2f);
            target = t;
            if (target)
            {
                nav.enabled = true;
                nav.SetDestination(target.position);
            }
            else
            {
                nav.enabled = false;
            }
        }
    }

}
    