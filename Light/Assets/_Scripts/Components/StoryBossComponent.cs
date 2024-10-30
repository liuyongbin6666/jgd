using System;
using GameData;
using GMVC.Core;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Components
{
    public class StoryBossComponent : MonoBehaviour
    {
        public bool IsDeath => Enemy == null || Enemy.IsUnityNull() || Enemy.IsDeath;
        [LabelText("Boss控件")] public EnemyComponent Enemy;
        [LabelText("打败奖励")] public GameObject[] Rewards;
        public readonly UnityEvent OnSeekEvent = new();

        //注意这个触发器是Boss初始调用的，千万别在触发器设置成完成，否这方法会变成完成才触发
        public void StoryStart(PlotComponentBase plot)
        {
            if(!Enemy || Enemy.IsUnityNull())
            {
                OnDeathAction();//todo: 这是容错，因为有可能玩家已经把怪物打死了！！
                return;
            }
            Enemy.Display(true);
            Enemy.OnDeathEvent.AddListener(OnDeathAction);
            Enemy.VisionActive.OnActiveEvent.AddListener(OnSeekAction);

            void OnSeekAction(bool isActive)
            {
                if (!isActive) return;
                if (plot.IsStoryFinalized) return;
                OnSeekEvent.Invoke();
                Game.MessagingManager.Send(GameEvent.Story_Boss_Battle);
            }
        }

        void OnDeathAction()
        {
            foreach (var go in Rewards)
            {
                go.transform.position = Enemy.transform.position;
                go.Display(true);
                Game.MessagingManager.Send(GameEvent.Story_Boss_Death);
            }
        }
    }
}