using GMVC.Core;
using GMVC.Utls;
using UnityEngine;

/// <summary>
/// 墓碑控件
/// </summary>
public class GraveComponent : GameItemBase
{
    [SerializeField] GameObject _boss;
    public override GameItemType Type => GameItemType.Grave;

    public override void Invoke(PlayableUnit player)
    {
        Game.SendEvent(GameEvent.Story_Show);
        _boss.Display(true);
        "Boss 出现!".Log(this);
    }
}