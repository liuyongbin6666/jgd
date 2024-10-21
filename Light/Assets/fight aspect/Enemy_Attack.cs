using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using System;

public class Enemy_Attack : MonoBehaviour
{
    public Rigidbody rig;
    public NavMeshAgent nav;
    public Transform player;
    public Transform target;
    public float startTime;
    public float attackRate=1f;
    public GameObject bullet;
    [SerializeField,LabelText("请选择射击方式")] public AttackStyle attackStyle;
    public bool isTrack;
    public void Init()
    {
        isTrack = false;
        nav.updateRotation = false;
    }
    private void Start()
    {
        //Init();
    }
    private void Update()
    {
        Shot();
    }
    public void Shot()
    {
        if (Time.time > startTime + attackRate)
        {
            startTime = Time.time;
                GameObject bullet = Instantiate(this.bullet, transform.position, Quaternion.identity);
                bullet.GetComponent<BulletComponent>().Init(player, attackStyle);
           
        }
    }
    //private void OnTriggerEnter(Collider other)//玩家进入攻击范围
    //{

    //    if (other.tag == "Player")
    //    {
    //        StartCoroutine(UpdateTarget(player.transform));
    //    }
    //}
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.tag == "Player"&& !isTrack)
    //    {
    //        StartCoroutine(UpdateTarget(player.transform));
    //    }
    //}
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
        target = null;
        collision.transform.GetComponent<HealthBarComponent>().GetHit(2f);
        collision.transform.GetComponent<HealthBarComponent>().UpdateContent();
    }

    //设置速度
    public void SetSpeed(float speed) => nav.speed = speed;
    IEnumerator UpdateTarget(Transform t)//获取当前目标位置
    {
        target = t;
        while (true)
        {           
            if (target!=null)
            {
                nav.enabled = true;
                nav.SetDestination(target.position);
                isTrack = true;
            }
            else if(target==null)
            {
                nav.enabled = false;
                isTrack = false;
                yield break;
            }
            yield return new WaitForSeconds(0.2f);
            //target = t;
        }
    }

}
    