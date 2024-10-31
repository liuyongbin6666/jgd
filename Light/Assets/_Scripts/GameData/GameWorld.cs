using GMVC.Core;
using GMVC.Utls;
using UnityEngine;

namespace GameData
{
    public class GameWorld
    {
        public enum GameStates
        {
            Start,
            Playing,
            End
        }
        public GameStates Status { get; private set; }
        public GameStage Stage { get; private set; }
        public int StageIndex { get; private set; }

        public void Start()
        {
            State_Set(GameStates.Start);
            Game.SendEvent(GameEvent.Game_Start);
        }

        public void End()
        {
            State_Set(GameStates.End);
            Game.SendEvent(GameEvent.Game_End);
        }

        void State_Set(GameStates state) => Status = state;

        public void StartGameStage()
        {
            Stage.Stage_Start();
            State_Set(GameStates.Playing);
            Game.SendEvent(GameEvent.Game_Playing);
        }
        public void SetGameStage(PlayableUnit player, StageStory stageStory)
        {
            Stage = new GameStage(player, stageStory);
            Game.SendEvent(GameEvent.Game_Stage_Start);
        }

        public void NextGameStage() => StageIndex++;
    }

    public class GameTag
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string AnimTrigger = "AnimTrigger";
        public const string AnimInt = "AnimInt";
        public const string PlayerSaveString = "PlayerSaveString";
    }
    public class GameEvent
    {
        public const string GameItem_Interaction = "GameItem_Interaction";// 游戏物品交互
        public const string Stage_StageTime_Update = "Stage_StageTime_Update";
        public const string Stage_Lose= "Stage_Lose";// 关卡结束
        public const string Stage_Complete= "Stage_Complete";// 关卡结束
        public const string Game_Stage_Start = "Game_Stage_Start";// 游戏关卡开始

        //玩家
        public const string Player_Lantern_Update = "Player_Lantern_Update";// 玩家灯笼更新
        public const string Player_Panic_Pulse = "Player_Panic_Pulse";//恐慌心跳
        //public const string Player_Panic_Finalize = "Player_Panic_Finalize";//恐慌心跳结束触发
        //public const string Player_IsDeath = "Player_IsDeath";//玩家死亡
        public const string Player_Hp_Update = "Player_Hp_Update";//玩家血量更新
        public const string Spell_Add = "Spell_Add";//玩家法术添加

        //环境
        public const string Env_Lightning = "Env_Lightning";// 闪电
        public const string Env_Rain_Update= "Env_Rain_Update";// 下雨

        //游戏
        public const string Game_Start = "Game_Start";
        public const string Game_End = "Game_End";
        public const string Game_Playing = "Game_Playing";
        public const string Game_Paused = "Game_Paused";// 游戏暂停
        public const string Game_Resume = "Game_Resume";

        //故事
        public const string Story_Lines_Send = "Story_Lines_Send"; // 游戏故事行发送
        public const string Story_Dialog_Send = "Story_Dialog_Send"; // 游戏故事对话发送
        public const string Story_Begin = "Story_Begin"; // 故事开始
        public const string Story_End = "Story_End"; // 游戏故事结束

        public const string Story_Soul_Interactive = "Story_Soul_Interactive";// npc交互
        public const string Story_Soul_Inactive = "Story_Soul_Inactive";// npc未激活信息
        public const string Story_Boss_Battle = "Story_Boss_Battle";// boss战斗
        public const string Story_Boss_Death = " Story_Boss_Death";//boss死亡

        //法术
        public const string Spell_Charge= "Spell_Charge";//法术补充
        public const string Spell_Cast= "Spell_Cast";//法术释放

        //战斗
        public const string Battle_Spell_On_Player = "Battle_Spell_On_Player"; //战斗法术对玩家伤害
        public const string Battle_Skeleton_Death = "Battle_Skeleton_Death"; // 战斗单位死亡
        public const string Battle_Bullet_Explode = "Battle_Bullet_Explode";// 子弹爆炸
    }
}