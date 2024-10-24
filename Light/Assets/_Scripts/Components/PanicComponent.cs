using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Components
{
    public class PanicComponent : MonoBehaviour
    {
        [SerializeField,LabelText("恐慌心跳次数")]int _pulseTimes = 5;
        [SerializeField,LabelText("间隔")]float _interval = 1f;
        public readonly UnityEvent<int> OnPulseTrigger = new();
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
                OnPulseTrigger.Invoke(pulse);
                yield return new WaitForSeconds(_interval);
                pulse--;
            }
            OnPulseComplete.Invoke();
        }

        public void StopIfPanic() => StopAllCoroutines();
    }
}