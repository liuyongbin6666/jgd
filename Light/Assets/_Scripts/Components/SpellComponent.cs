using GameData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components
{
    /// <summary>
    /// 魔法控件
    /// </summary>
    public class SpellComponent : GameItemBase
    {
        [SerializeField,LabelText("类型")] Spell _spell;
        [SerializeField, LabelText("使用次数")] int _times;
        public override GameItemType Type => GameItemType.Firefly;

        public override void Invoke(PlayableUnit player)
        {
            player.AddSpell(_spell, _times);
            Destroy(gameObject);
        }
    }
}