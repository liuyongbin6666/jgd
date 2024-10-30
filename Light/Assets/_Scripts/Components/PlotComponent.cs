using System.Collections;
using System.Collections.Generic;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Components
{
    /// <summary>
    /// 情节控件 - IsFinalize 控制是否触发"完成"
    /// </summary>
    public class PlotComponent : PlotComponentBase
    {
        [LabelText("情节(自动)完成")]public bool IsFinalize;
        [SerializeField,LabelText("完结奖励")]List<GameObject> Rewards;
        protected override bool DisableLines => false;

        protected override void OnBegin()
        {
            StartCoroutine(BeginRoutine());
            return;

            IEnumerator BeginRoutine()
            {
                yield return null;
                yield return new WaitUntil(() => IsFinalize);
                if (State == States.Begin) Interaction();
            }
        }

        protected override void OnFinalization()
        {
            foreach (var reward in Rewards)
            {
                reward.Display(true);
                reward.transform.position = transform.position.ChangeZ(-1);
            }
        }
    }
}