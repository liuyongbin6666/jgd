using System;
using GameData;
using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace Components
{
    /// <summary>
    /// 萤火虫控件
    /// </summary>
    public class FireflyComponent : GameItemBase
    {
        [SerializeField, LabelText("虫灯值")] int _lantern = 1;
        [SerializeField] Firefly[] _fireflies;
        public int Count => _fireflies.Length;
        public override GameItemType Type => GameItemType.Firefly;
        
        public void RandomSet()
        {
            var f = _fireflies.WeightPick();
            foreach (var firefly in _fireflies)
            {
                firefly._obj.SetActive(f == firefly);
                if (f == firefly) _lantern = firefly._lantern;
            }
        }
        public void Set(int index)
        {
            for (int i = 0; i < _fireflies.Length; i++)
            {
                var firefly = _fireflies[i];
                firefly._obj.SetActive(i == index);
                if (i == index) _lantern = firefly._lantern;
            }
        }
        public override void Invoke(PlayableUnit player)
        {
            player.AddLantern(_lantern);
            Destroy(gameObject);
        }
        [Serializable] class Firefly:IWeightElement
        {
            [LabelText("虫灯值")] public int _lantern = 1;
            public GameObject _obj;
            [LabelText("权重")]public int weight;
            public int Weight => weight;
        }
    }
}