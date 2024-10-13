using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class StageTimeComponent :CountdownComponent
{
    [SerializeField,LabelText("心跳次数")] int pulseTimes;
    [SerializeField,LabelText("关卡时长")] float duration;

    protected override int PulseTimes => pulseTimes;

    protected override float Duration => duration;
}
