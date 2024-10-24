using System;
using Components;
using GMVC.Conditions;
using GMVC.Utls;
using Sirenix.OdinInspector;
using UnityEngine;
using Utls;
using Object = UnityEngine.Object;

namespace GameData
{
    /// <summary>
    /// 游戏操作单位
    /// </summary>
    public class PlayableUnit : ModelBase
    {
        /// <summary>
        /// 灯笼值
        /// </summary>
        public int Lantern => Firefly.Value;
        public PlayerControlComponent PlayerControl { get; }
        public bool IsMoving => PlayerControl.IsMoving;
        Player Player { get; }
        ConValue Hp => Player.Hp;
        ConValue Mp => Player.Mp;
        ConValue Firefly => Player.Firefly;
        public PlayableUnit(Player player,PlayerControlComponent playerControl)
        {
            Player = player;
            PlayerControl = playerControl;
            PlayerControl.Init();
            PlayerControl.Lantern_Update(Lantern);
            PlayerControl.OnLanternTimeout.AddListener(OnLanternTimeout);
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
            if(Player.IsDeath) SendEvent(GameEvent.Player_IsDeath);
        }

        // 当游戏物品交互
        void OnGameItemInteractive(GameItemBase gameItem)
        {
            gameItem.Invoke(this);
            SendEvent(GameEvent.GameItem_Interaction, gameItem.Type);// 游戏物品交互，发射了枚举入参为游戏物品类型
        }
        //当恐慌心跳, times = 剩余次数, 1为最后一次
        void OnPanicPulse(int times)
        {
            //Log(times);
            SendEvent(GameEvent.Player_Panic_Pulse, times);
        }
        //当恐慌结束
        void OnScaryFinalized()
        {
            Log();
            SendEvent(GameEvent.Player_Panic_Finalize);
        }
        //当灯笼减弱时间触发
        void OnLanternTimeout()
        {
            LanternUpdate(Lantern - 1);
            //Log(nameof(OnLanternTimeout)+$" : {lantern}");
        }

        public void AddLantern(int value) => LanternUpdate(Lantern + value);
        //灯笼更新
        void LanternUpdate(int value)
        {
            Firefly.Set(value);
            if (Lantern == 1)
            {
                PlayerControl.StartPanic();// 开始恐慌
            }
            Log($"value = {value}");
            PlayerControl.Lantern_Update(Lantern);
            SendEvent(GameEvent.Player_Lantern_Update);
        }
        public void Move(Vector3 direction) => PlayerControl.axisMovement = direction.ToXY();
        public void Enable(bool enable) => PlayerControl.Display(enable);
    }

    public class Player
    {
        public ConValue Hp { get; }
        public ConValue Mp { get; }
        public ConValue Firefly { get; }
        public bool IsDeath => Hp.IsExhausted;
        public Player(ConValue hp, ConValue mp, ConValue firefly)
        {
            Hp = hp;
            Mp = mp;
            Firefly = firefly;
        }
    }
    /// <summary>
    /// 法术
    /// </summary>
    [Serializable]public struct Spell
    {
        public enum Types
        {
            [InspectorName("普通")]Normal,
            [InspectorName("火")]Fire,
            [InspectorName("冰")]Ice
        }
        [LabelText("类型")]public Types Type;
        [LabelText("伤害")]public int Damage;
        [LabelText("等级")]public int Level;
        [LabelText("延迟")]public float Delay;
        [LabelText("击退")]public float force;

        public Spell(Types type, int damage, int level, float force, float delay)
        {
            Type = type;
            Damage = damage;
            Level = level;
            this.force = force;
            Delay = delay;
        }
    }
}