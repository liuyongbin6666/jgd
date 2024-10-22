using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace fight_aspect {
    public class HealthBarComponent : MonoBehaviour
    {
        public float content;//可以是蓝量也可以是血量
        public float Maxcontent;
        public Image hp_Main;
        public float hurtSpeed = 0.005f;
        public float healSpeed = 0.006f;
        public float hurt_duration = 2f;
        public float heal_duration = 2f;
        [SerializeField, LabelText("当前滚动条类别")] SliderType st;
        public void Init(float hp)
        {
            Maxcontent = hp;
            content = Maxcontent;
        }
        public virtual void GetHit(float t)//
        {
            content = content - t;
        }
        public virtual void GetHeal(float t)
        {
            content = content + t;
        }
        public void UpdateContent()
        {
            CheckContent();
            hp_Main.fillAmount = content / Maxcontent;
        }
        void CheckContent()
        {
            if (content < 0)
            {
                content = 0;
            }
            if(content>Maxcontent)
            {
                content = Maxcontent;
            }
        }
        public void GetHealSlowly(float s=0.006f/*s为每过值为healSpeed的时间增加的血量*/)//缓慢加血
        {
            StartCoroutine(Healing(s));
        }
        public virtual IEnumerator Healing(float s)
        {
            float startTime = Time.time;
            while (Time.time < heal_duration)
            {
                content += s;
                UpdateContent();
                yield return new WaitForSeconds(healSpeed);
            }
        }
        public void GetPoison(float s= 0.005f/*s为每过值为hurtSpeed的时间减少的血量*/)//缓慢扣血或中毒
        {
            StartCoroutine(Poisoning(s));
        }
        public virtual IEnumerator Poisoning(float s)
        {
            float startTime = Time.time;
            while(Time.time<hurt_duration)
            {
                content -= s;
                UpdateContent();
                yield return new WaitForSeconds(hurtSpeed);
            }
        }
    }
    enum SliderType
    {
        [SerializeField,LabelText("血量")]hp,
        [SerializeField, LabelText("蓝量")] mana
    }
}