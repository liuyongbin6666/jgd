using System;
using System.Collections.Generic;
using System.Linq;
using Config;
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
        [SerializeField,LabelText("法术配置")] SpellSo _spellSo;
        [SerializeField,LabelText("法术"),ValueDropdown(nameof(GetSpells))] string _spellName;
        [SerializeField, LabelText("使用次数")] int _times;
        public override GameItemType Type => GameItemType.Firefly;
        IEnumerable<string> GetSpells() => _spellSo.Spells.Select(s => s.SpellName);

        public override void Invoke(PlayableUnit player)
        {
            player.AddSpell(_spellSo.GetSpell(_spellName).Value, _times);
            Destroy(gameObject);
        }
    }
}