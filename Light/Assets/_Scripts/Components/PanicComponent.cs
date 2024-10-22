using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace Components
{
    public class PanicComponent : CountdownComponent
    {
        [SerializeField,LabelText("恐慌心跳次数")]int _pulseTimes = 5;
        [SerializeField,LabelText("持续时间")]float _duration = 5;
        protected override int PulseTimes => _pulseTimes;
        protected override float Duration => _duration;

        public void Init()
        {
        }
        public void StartPanic() => StartCountdown();
        public void StopIfPanic() => StopCountdown();
    }
}