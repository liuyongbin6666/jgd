using System;
using Components;
using GameData;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using fight_aspect;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "主角so", menuName = "配置/主角配置")]
    public class PlayerCfgSo : ScriptableObject
    {
        [SerializeField, LabelText("移动速度")] public float moveSpeed = 1.5f;
        [SerializeField, LabelText("虫灯")] Lantern lantern;
        [SerializeField, LabelText("恐慌")] Panic panic;
        [SerializeField, LabelText("攻击")] Attack attack;
        [Button("保存配置")]public void Save(PlayerControlComponent com)
        {
            moveSpeed = com.moveSpeed;
            lantern.Set(com);
            panic.Set(com._panicCom);
            attack.Set(com.magicStaff.attackComponent);
        }
        [Button("加载配置")]public void Load(PlayerControlComponent com)
        {
            com.moveSpeed = moveSpeed;
            lantern.Load(com);
            panic.Load(com._panicCom);
            attack.Load(com.magicStaff.attackComponent);
        }
        [Serializable] class Lantern
        {
            [LabelText("虫灯最大值")] public int _maxLantern = 5;
            [LabelText("虫灯最小值")] public int _minLantern = 1;
            [LabelText("虫灯维持秒数")] public float _lastingPerFirefly = 3f;
            [LabelText("虫灯范围等级")] public List<LightSettings> lanternSettings; // 设置数组

            public void Set(PlayerControlComponent com)
            {
                lanternSettings = com._lantern.LanternVisionLevel.settings;
                _lastingPerFirefly = com._lantern._lastingPerFirefly;
                _maxLantern = com._maxLantern;
                _minLantern = com._minLantern;
            }
            public void Load(PlayerControlComponent com)
            {
                com._lantern.LanternVisionLevel.settings = lanternSettings;
                com._lantern._lastingPerFirefly = _lastingPerFirefly;
                com._maxLantern = _maxLantern;
                com._minLantern = _minLantern;
            }
        }
        [Serializable] class Panic
        {
            [LabelText("恐慌心跳次数")] public int _pulseTimes = 5;
            [LabelText("间隔")] public float _interval = 1f;

            public void Set(PanicComponent com)
            {
                _pulseTimes = com._pulseTimes;
                _interval = com._interval;
            }
            public void Load(PanicComponent com)
            {
                com._pulseTimes = _pulseTimes;
                com._interval = _interval;
            }
        }
        [Serializable] class Attack
        {
            [LabelText("攻击CD")] public float cd = 1f;
            public void Set(AttackComponent com)
            {
                cd = com.cd;
            }
            public void Load(AttackComponent com)
            {
                com.cd = cd;
            }
        }
    }
}