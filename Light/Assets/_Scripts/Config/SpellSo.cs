using System;
using System.Linq;
using GameData;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "技能配置", menuName = "配置/技能配置")]
    public class SpellSo :ScriptableObject
    {
        public Spell[] Spells => list.Select(s => s.Spell).ToArray();
        [SerializeField] SpellField[] list;
        public Spell? GetSpell(string spellName)=> Spells.FirstOrDefault(s => s.SpellName == spellName);
        public Sprite GetSprite(string spellName) => list.FirstOrDefault(s => s.Spell.SpellName == spellName).Sprite;
        [Serializable] class SpellField
        {
            public Sprite Sprite;
            public Spell Spell;
        }
    }
}