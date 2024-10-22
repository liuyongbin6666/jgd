using System.Collections.Generic;
using UnityEngine;

namespace fight_aspect
{
    public class PlayerController : MonoBehaviour
    {
        SphereCollider col;//视野范围碰撞器
        public GameObject bulletPrefab;//子弹的资源
        public List<Transform> enemyList;//攻击列表，因为我不太理解组件所以只能写Transform类
        public Transform bulletGarbage;//子弹父类，便于管理
        public int attackcount;//单次攻击的目标数量
        public float startTime;//记录每次攻击的时间
        public float attackRate;//攻击频率
        public float speed;
        public HealthBarComponent hp;
        void Init() 
        {
            hp = GetComponent<HealthBarComponent>();
            hp.Init(20); 
        }
        void Start()
        {
            Init();
        }
        void Update()
        {
            Move();
        }
        public void AttackListAdd(Transform t)//加入攻击列表
        {
            if (t.tag == "Enemy" && !enemyList.Contains(t.transform))
            {
                enemyList.Add(t.transform);
            }
        }
        public void AttackListRemove(Transform t)//脱离攻击列表
        {
            if (t.tag == "Enemy" && enemyList.Contains(t.transform))
            {
                enemyList.Remove(t.transform);
            }
        }
        void Move()//简单的移动，只是用来模拟用的，与功能无关
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direct = new Vector3(horizontal, 0, vertical);
            if(direct!=Vector3.zero)
            {
                transform.position += direct * Time.deltaTime*speed;
            }
        }
        void SetViewRadius(float r)//设置攻击范围
        {
            col.radius = r;
        }
        private void OnTriggerEnter(Collider other)//记录进入的敌人
        {
            AttackListAdd(other.transform);
        }
        private void OnTriggerStay(Collider other)//攻击敌人
        {
            if(enemyList.Count!=0)
            {
                Attack();
            
            }
        }
        private void OnTriggerExit(Collider other)//删除离开范围的敌人
        {
            AttackListRemove(other.transform);
        }
        void SetAttackRate(float t)//设置攻击频率
        {
            attackRate = t;
        }
        public void Attack()//攻击
        {
            if (Time.time > startTime + attackRate)
            {Debug.Log("攻击怪物");
                float count = attackcount;
                foreach (var enemy in enemyList)
                {
                    if (enemy != null)
                    {
                        GameObject bullet=Instantiate(bulletPrefab,transform.position,Quaternion.identity);
                        bullet.transform.SetParent(bulletGarbage);
                        bullet.GetComponent<BulletComponent>().Target = enemy;
                        count--;
                    }
                    if (count <= 0)
                        break;
                }
                startTime = Time.time;
            }
        }

    }
}
