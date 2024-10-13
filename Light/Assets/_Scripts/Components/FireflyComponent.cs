using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 萤火虫控件
/// </summary>
public class FireflyComponent : GameItemBase
{
    [SerializeField,LabelText("虫灯值")] int _lantern = 1;
    public override GameItemType Type => GameItemType.Firefly;

    public override void Invoke(PlayableUnit player)
    {
        player.AddLantern(_lantern);
        Destroy(gameObject);
    }
}