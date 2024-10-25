using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Components
{
    public class PanicComponent : MonoBehaviour
    {
        [LabelText("恐慌心跳次数")]public int _pulseTimes = 5;
        [LabelText("间隔")]public float _interval = 1f;
        public readonly UnityEvent<int,int> OnPulseTrigger = new();
        public readonly UnityEvent OnPulseComplete = new();

        public void StartPanic()
        {
            StopAllCoroutines();
            StartCoroutine(PanicRoutine());
        }

        IEnumerator PanicRoutine()
        {
            var pulse = _pulseTimes;
            while (pulse > 0)
            {
                OnPulseTrigger.Invoke(pulse, _pulseTimes);
                yield return new WaitForSeconds(_interval);
                pulse--;
            }
            OnPulseComplete.Invoke();
        }

        public void StopIfPanic() => StopAllCoroutines();
    }
}