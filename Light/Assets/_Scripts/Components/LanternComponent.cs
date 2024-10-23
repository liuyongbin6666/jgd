using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace Components
{
    /// <summary>
    /// 灯笼控件
    /// </summary>
    public class LanternComponent : CountdownComponent
    {
        [SerializeField,LabelText("减弱时长")] float _lastingPerFirefly = 3f;
        [SerializeField] VisionLevelComponent _visionLevel;
        protected override int PulseTimes => 1;
        protected override float Duration => _lastingPerFirefly;
        public float VisionRadius => _visionLevel.VisionRadius;
        public void Init()
        {
            StartCountdown();
        }
        public void SetVisionLevel(int level)
        {
            _visionLevel.LoadLightLevel(level,out var isMaxLevel);
            if (isMaxLevel) StartCountdown(true);
        }
    }
}