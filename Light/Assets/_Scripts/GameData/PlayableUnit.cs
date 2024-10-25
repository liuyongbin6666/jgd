using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using fight_aspect;
using GMVC.Conditions;
using GMVC.Core;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;

namespace GameData
{
    /// <summary>
    /// 游戏操作单位
    /// </summary>
    public class PlayableUnit : ModelBase
    {
        const int Lantern_Min = 1;

        /// <summary>
        /// 灯笼值
        /// </summary>
        public int Lantern => Firefly.Value;
        public PlayerControlComponent PlayerControl { get; }
        public bool IsMoving => PlayerControl.IsMoving;
        Player Player { get; }
        public ConValue Hp => Player.Hp;
        public ConValue Firefly => Player.Firefly;
        public List<Spell> Spells => Player.Spells;
        public int SelectedSpellIndex => Player.SpellIndex;

        public PlayableUnit(Player player,PlayerControlComponent playerControl)
        {
            Player = player;
            PlayerControl = playerControl;
            PlayerControl.Init();
            PlayerControl.OnCastSpell += CastSpell;
            PlayerControl.Lantern_Update(Lantern);
            PlayerControl.OnLanternPulse.AddListener(()=>AddLantern(-1));
            PlayerControl.OnPanicFinalize.AddListener(OnScaryFinalized);
            PlayerControl.OnPanicPulse.AddListener(OnPanicPulse);
            PlayerControl.OnGameItemTrigger.AddListener(OnGameItemInteractive);
            PlayerControl.OnSpellImpact.AddListener(OnSpellImpact);
        }

        void OnSpellImpact(Spell spell)
        {
            var damage = spell.Damage;
            if (damage < 0)
            {
                Debug.LogError($"伤害值异常! = {damage}");
                return;
            }
            Hp.Add(-damage);
            SendEvent(GameEvent.Battle_Spell_On_Player, spell.Damage); 
            SendEvent(GameEvent.Player_Hp_Update);
            if(Player.IsDeath) SetDeath();
        }

        void SetDeath()
        {
            PlayerControl.Die();
            SendEvent(GameEvent.Player_IsDeath);
        }

        // 当游戏物品交互
        void OnGameItemInteractive(GameItemBase gameItem)
        {
            gameItem.Invoke(this);
            SendEvent(GameEvent.GameItem_Interaction, gameItem.Type);// 游戏物品交互，发射了枚举入参为游戏物品类型
        }
        //当恐慌心跳, times = 剩余次数, 1为最后一次
        void OnPanicPulse(int times,int max)
        {
            //Log(times);
            SendEvent(GameEvent.Player_Panic_Pulse, times, max);
        }
        //当恐慌结束
        void OnScaryFinalized()
        {
            Log();
            SendEvent(GameEvent.Player_Panic_Finalize);
            SetDeath();
        }

        public void AddLantern(int value)
        {
            //灯笼更新
            Firefly.Set(Lantern + value);
            if (Lantern == Lantern_Min)
            {
                PlayerControl.StartPanic(); // 开始恐慌
            }
            //Log($"value = {value}");
            PlayerControl.Lantern_Update(Lantern);
            SendEvent(GameEvent.Player_Lantern_Update);
        }

        public void Move(Vector3 direction) => PlayerControl.axisMovement = direction.ToXY();
        public void Enable(bool enable) => PlayerControl.Display(enable);

        Spell CastSpell()
        {
            //如果没有选择法术，就使用默认法术
            if (SelectedSpellIndex < 0) return Game.DefaultSpell;
            //特殊法术，需要发送事件
            var spellIndex = SelectedSpellIndex;
            Player.CastSpell(spellIndex, out bool isFinish, out var remain, out var max);
            if (isFinish) SendEvent(GameEvent.Battle_Spell_Finish, spellIndex);
            else SendEvent(GameEvent.Battle_Spell_Update, spellIndex, remain, max);
            return Player.CurrentSpell;
        }

        public void AddSpell(Spell spell, int times)
        {
            Player.AddSpell(spell, times);
            SendEvent(GameEvent.Player_Spell_Add, spell, times);
        }
        public void ChargeSpell(int spellIndex)
        {
            if (Lantern < Lantern_Min) return;
            if (Player.IsSpellMax(spellIndex)) return;
            AddLantern(-1);
            var m = Player.ChargeSpell(spellIndex);
            Player.SelectSpell(spellIndex);
            SendEvent(GameEvent.Battle_Spell_Update, spellIndex, m.remain, m.max);
        }
    }

    public class Player
    {
        public ConValue Hp { get; }
        public ConValue Firefly { get; }
        List<Magic> Magics { get; } = new();
        /// <summary>
        /// -1 = 没有法术
        /// </summary>
        public int SpellIndex => CurrentSpellIndex();
        int _selectedSpellIndex;
        int CurrentSpellIndex()
        {
            for (var i = 0; i < Magics.Count; i++)// 选择第一个有次数的法术
            {
                var magic = Magics[i];
                if (_selectedSpellIndex < 0 && magic.Times > 0)
                {
                    _selectedSpellIndex = i;
                    return _selectedSpellIndex;
                }
                if (_selectedSpellIndex == i && magic.Times > 0) return _selectedSpellIndex;
            }
            _selectedSpellIndex = -1;
            return _selectedSpellIndex;
        }
        public List<Spell> Spells => Magics.Where(m => m.Times > 0).Select(s => s.Spell).ToList();
        public bool IsDeath => Hp.IsExhausted;
        public Spell CurrentSpell => Magics[_selectedSpellIndex].Spell;

        public Player(ConValue hp, ConValue firefly)
        {
            Hp = hp;
            Firefly = firefly;
        }
        public void AddSpell(Spell spell, int times)//添加法术，自动选择
        {
            for (var i = 0; i < Magics.Count; i++)
            {
                var m = Magics[i];
                if (m.Spell.SpellName != spell.SpellName) continue;
                m.AddTimes(times);
                _selectedSpellIndex = i;// 选择这个法术
                return;
            }
            _selectedSpellIndex = Magics.Count;
            Magics.Add(new Magic(spell, times));
        }

        public bool IsSpellMax(int index)
        {
            var magic = Magics[index];
            return magic.Times == magic.Max;
        }

        public (int remain,int max) ChargeSpell(int index)
        {
            Magics[index].SetMax();
            return (Magics[index].Times, Magics[index].Max);
        }
        public void CastSpell(int spellIndex, out bool isFinish, out int remain, out int max)
        {
            var spell = Magics[spellIndex];
            isFinish = spell.Cast();
            remain = spell.Times;
            max = spell.Max;
        }
        public void SelectSpell(int index) => _selectedSpellIndex = index;

        class Magic
        {
            public Spell Spell { get; }
            public int Max { get; }
            public int Times { get; private set; }
            public void AddTimes(int times) => Times += times;
            public bool Cast()
            {
                --Times;
                Times = Math.Max(0, Times);
                //返回是否消耗完
                return Times <= 0;
            }

            public Magic(Spell spell, int times)
            {
                Spell = spell;
                Max = Times = times;
            }
            public void SetMax() => Times = Max;
        }

    }
    /// <summary>
    /// 法术
    /// </summary>
    [Serializable]public struct Spell
    {
        [LabelText("名字")]public string SpellName;
        [LabelText("伤害")]public int Damage;
        [LabelText("速度")]public float Speed;
        [LabelText("等级")]public int Level;
        [LabelText("延迟")]public float Delay;
        [LabelText("击退")]public float force;
        [LabelText("子弹")]public BulletTracking Tracking;
        [LabelText("范围伤害")]public bool RangeDamage;
    }
}