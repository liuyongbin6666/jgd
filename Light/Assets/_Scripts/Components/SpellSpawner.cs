using System;
using Config;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace Components
{
    public class SpellSpawner : RandomObjectSpawner<SpellComponent>
    {
        [SerializeField] SpellField[] Spells;
        [SerializeField, LabelText("法术配置")] public SpellSo _spellSo;
        IEnumerable<string> GetSpells() => _spellSo.Spells.Select(s => s.SpellName);
        protected override void Get(SpellComponent spell)
        {
            var s = Spells.WeightPick();
            spell._spellName = s._spellName;
            spell._times = s.times;
        }
        protected override void Recycle(SpellComponent obj) { }
        [Serializable]class SpellField : IWeightElement
        {
            public int _weight = 1;
            [SerializeField, LabelText("法术"), ValueDropdown("@((SpellSpawner)$property.Tree.WeakTargets[0]).GetSpells()")] public string _spellName;
            public int times = 10;
            public int Weight => _weight;
        }

    }
}