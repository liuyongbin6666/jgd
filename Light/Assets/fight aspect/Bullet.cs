using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Transform target;
    public void Init()
    {
    }
    private void Start()
    {
        Init();
    }
    private void Update()
    {
        if (target != null)
        {
            gameObject.transform.position = Vector3.MoveTowards(transform.position, target.position, speed*Time.deltaTime);
        }
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
        if(other.tag=="Enemy")
        {
            Destorythis();
        }
    }
}
