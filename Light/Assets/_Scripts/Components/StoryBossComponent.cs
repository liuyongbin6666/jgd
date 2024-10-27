using System;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    public class StoryBossComponent : MonoBehaviour
    {
        public bool IsDeath => Enemy == null;
        [LabelText("Boss控件")] public EnemyComponent Enemy;
        [LabelText("打败奖励")] public GameObject[] Rewards;
        public readonly UnityEvent OnSeekEvent = new();

        //注意这个触发器是Boss初始调用的，千万别在触发器设置成完成，否这方法会变成完成才触发
        public void StoryStart(PlotComponentBase plot)
        {
            Enemy.Display(true);
            Enemy.OnDeathEvent.AddListener(OnDeathAction);
            Enemy.VisionActive.OnActiveEvent.AddListener(OnSeekAction);

            void OnSeekAction(bool isActive)
            {
                if (!isActive) return;
                if (plot.IsStoryFinalized) return;
                OnSeekEvent.Invoke();
            }
        }

        void OnDeathAction()
        {
            foreach (var go in Rewards)
            {
                go.transform.position = Enemy.transform.position;
                go.Display(true);
            }
        }
    }
}