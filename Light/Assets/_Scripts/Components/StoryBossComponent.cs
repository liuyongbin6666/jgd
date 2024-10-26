using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components
{
    public class StoryBossComponent : MonoBehaviour
    {
        public bool IsDeath => Enemy == null;
        [SerializeField,LabelText("播放文本")] bool sendDialog;
        [LabelText("Boss控件")] public EnemyComponent Enemy;
        bool PlayedLine { get; set; }
        
        //注意这个触发器是Boss初始调用的，千万别在触发器设置成完成，否这方法会变成完成才触发
        public void StoryStart(PlotComponentBase plot)
        {
            Enemy.Display(true);
            if (sendDialog) Enemy.VisionActive.OnActiveEvent.AddListener(OnSeekAction);
            return;

            void OnSeekAction(bool isActive)
            {
                if (!isActive) return;
                if (PlayedLine) return;
                if (plot.IsStoryFinalized) return;
                PlayedLine = true;
                plot.SendLines();
            }
        }
    }
}