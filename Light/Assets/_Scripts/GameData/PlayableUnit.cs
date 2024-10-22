using Components;
using GMVC.Conditions;
using UnityEngine;
using Utls;

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
        public int Lantern { get; private set; }= 1;
        public PlayerControlComponent PlayerControl { get; }
        public bool IsMoving => PlayerControl.IsMoving;
        Player Player { get; }
        ConValue Hp => Player.Hp;
        ConValue Mp => Player.Mp;
        public PlayableUnit(Player player,PlayerControlComponent playerControl, int lantern, float lightStep)
        {
            Lantern = lantern;
            Player = player;
            PlayerControl = playerControl;
            PlayerControl.Init(lightStep);
            PlayerControl.Lantern(Lantern);
            PlayerControl.OnLanternTimeout.AddListener(OnLanternTimeout);
            PlayerControl.OnPanicFinalize.AddListener(OnScaryFinalized);
            PlayerControl.OnPanicPulse.AddListener(OnPanicPulse);
            PlayerControl.OnGameItemTrigger.AddListener(OnGameItemInteractive);
            PlayerControl.OnDamage.AddListener(OnDamage);
            PlayerControl.OnSpellImpact.AddListener(OnSpellImpact);
        }

        void OnSpellImpact(Spell spell)
        {
            Hp.Add(spell.Damage);
            SendEvent(GameEvent.Battle_Spell_On_Player, spell.Damage);
        }

        // 当被伤害
        void OnDamage(int damage)
        {
            if (damage < 0)
            {
                Log($"伤害值异常! = {damage}");
                return;
            }
            Hp.Add(-damage);
            SendEvent(GameEvent.Player_Hp_Update);
            if(Player.IsDeath) SendEvent(GameEvent.Player_IsDeath);
        }
        // 当游戏物品交互
        void OnGameItemInteractive(GameItemBase gameItem)
        {
            gameItem.Invoke(this);
            SendEvent(GameEvent.GameItem_Interaction, gameItem.Type);// 游戏物品交互，发射了枚举入参为游戏物品类型
        }

        //当恐慌心跳, times = 剩余次数
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
            LanternUpdate(--Lantern);
            //Log(nameof(OnLanternTimeout)+$" : {lantern}");
        }

        public void AddLantern(int value) => LanternUpdate(Lantern + value);
        //灯笼更新
        void LanternUpdate(int value)
        {
            Lantern = value;// 灯笼++
            if (Lantern <= 0)
            {
                Lantern = 0;
                PlayerControl.StartPanic();// 开始恐慌
            }
            Log($"value = {value}");
            PlayerControl.Lantern(Lantern);
            SendEvent(GameEvent.Player_Lantern_Update);
        }
        public void Move(Vector3 direction) => PlayerControl.axisMovement = direction.ToVector2();
    }

    public class Player
    {
        public ConValue Hp { get; }
        public ConValue Mp { get; }
        public bool IsDeath => Hp.IsExhausted;
        public Player(ConValue hp, ConValue mp)
        {
            Hp = hp;
            Mp = mp;
        }
    }
    /// <summary>
    /// 法术
    /// </summary>
    public struct Spell
    {
        public enum Types
        {
            Normal,
            Fire,
            Ice
        }
        public Types Type;
        public int Damage;
        public int Level;
        public Spell(Types type, int damage, int level)
        {
            Type = type;
            Damage = damage;
            Level = level;
        }
    }
}