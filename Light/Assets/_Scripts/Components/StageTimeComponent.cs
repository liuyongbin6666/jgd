using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Components
{
    public class StageTimeComponent :MonoBehaviour
    {
        public readonly UnityEvent OnSecond = new UnityEvent();
        Coroutine Co;
        
        public void StartService(bool force)
        {
            if(force) StopService();
            Co ??= StartCoroutine(Counting());
        }

        public void StopService()
        {
            if (Co == null) return;
            StopCoroutine(Co);
            Co = null;
        }

        IEnumerator Counting()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                OnSecond?.Invoke();
            }
        }
    }
}
