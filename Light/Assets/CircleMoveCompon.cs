using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class CircleMoveComponent : MonoBehaviour
{
    public Transform centerPoint;
    public float r=3f;
    public float maxR=3f;
    public float angle;
    public float w=15f;
    public float speed=2f;
    public Vector3 direct;
    public UnityEvent rotateToLeft;
    public UnityEvent rotateToRight;
    public void Init()
    {
        
    }
    private void Start()
    {
        rotateToLeft.AddListener(RotateToLeft);
        rotateToRight.AddListener(RotateToRight);
    }
    private void Update()
    {
        KeepCenter();
        if(Input.GetKey(KeyCode.J))
        {
            rotateToLeft?.Invoke();
        }
        if(Input.GetKey(KeyCode.K))
        {
            rotateToRight?.Invoke();
        }
    }
    public void SetRadius(float s)
    {
        r = s;
    }
    public void SetCenter(Transform target)
    {
        centerPoint = target;
    }
    public void KeepCenter()//保持
    {
        direct = (centerPoint.position-transform.position);
        direct.y = 0;
        float distance = direct.magnitude;
        direct = direct.normalized;
        if (distance <= maxR)
        {
            r = distance;
            angle = Mathf.Atan2(-direct.z, -direct.x)*180/(float)Math.PI;
            Debug.Log(angle);
            return;
        }
        else
        {
            transform.position += direct * Time.deltaTime * speed;
        }
    }
    public void Rotate(float s /*用于调整旋转方向,-1为顺时针，1为逆时针*/)
    {       
            angle +=s* w * Time.deltaTime;
            float x = centerPoint.position.x + r * Mathf.Cos(angle*((float)Math.PI/180));
            float z = centerPoint.position.z + r * Mathf.Sin(angle * ((float)Math.PI / 180));
            transform.position = new Vector3(x, transform.position.y, z);
            
    }
    public void RotateToLeft()
    {
        StartCoroutine(ToLeft());
    }
    public void RotateToRight()
    {
        StartCoroutine(ToRight());
    }
    public IEnumerator ToLeft()
    {
        while(angle>-180)
        {
            Rotate(-1);
            yield return null;
            if (angle > 175)
                yield break;   
        }
    }
    public IEnumerator ToRight()
    {
        while(angle<-5||angle>5)
        {
           Rotate(1);
           yield return null;
        }
    }

}
