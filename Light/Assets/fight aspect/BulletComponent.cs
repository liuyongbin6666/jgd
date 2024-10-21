using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletComponent : MonoBehaviour
{
    public float distance;
    public float currentDistance;
    public Vector3 targetPosition;
    public Vector3 currentPosition;
    public float speed;
    public Vector3 direct;
    public float startTime;
    public float duration=2f;//生命周期
    public AttackStyle attackStyle;
    public Transform target;
    public void Init(Transform target,AttackStyle attackStyle)
    {
        this.target = target;
        this.attackStyle = attackStyle;        
        targetPosition = target.position;
        currentPosition = transform.position;
        direct = (targetPosition-currentPosition).normalized;
        distance = (targetPosition-currentPosition).magnitude;
        currentDistance = distance;
        SetSpeed(2f);
        startTime = Time.time;
        Shot();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        //if(Time.time>startTime+duration)
        //{
        //    Destorythis();
        //}
    }
    public void Shot()
    {
        if(attackStyle==AttackStyle.direct)
        {
            StartCoroutine(ShotDirect());
        }
        if(attackStyle==AttackStyle.halftrack)
        {
            StartCoroutine(ShotHalfTrack());
        }
        if(attackStyle==AttackStyle.track)
        {
            StartCoroutine(ShotTrack());
        }
    }
    public IEnumerator ShotTrack()//追踪攻击
    {
        while(gameObject)
        {
            gameObject.transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            yield return 0f;
        }
        
    }
    public IEnumerator ShotDirect()//定向攻击
    {
        while (gameObject)
        {
            transform.position += direct * Time.deltaTime * speed;
            yield return 0f;
        }
    }
    public IEnumerator ShotHalfTrack()//半追踪
    {
        while(gameObject)
        {
            currentDistance = (targetPosition - currentPosition).magnitude;
            if(currentDistance>distance/2)
            {
                gameObject.transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                targetPosition = target.position;
                currentPosition = transform.position;
            }
            else
            {
                direct = (targetPosition - currentPosition).normalized;
                attackStyle = AttackStyle.direct;
                break;
            }
            yield return 0f;
        }
        Shot(); 
    }
    void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
    void SetSpeed(float speed) => this.speed = speed;
    public void Destorythis()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag==target.tag)
        {
            Destorythis();
        }
    }
}
public enum AttackStyle
{
    track,
    direct,
    halftrack
}
