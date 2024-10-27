using GMVC.Core;

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
        }
        public void End()
        {
            State_Set(GameStates.End);
        }

        void State_Set(GameStates state)
        {
            Status = state;
            Game.SendEvent(GameEvent.Game_StateChanged, state);
        }
        public void StartGameStage()
        {
            Stage.Stage_Start();
            State_Set(GameStates.Playing);
        }
        public void SetGameStage(PlayableUnit player, StageStory stageStory)
        {
            Stage = new GameStage(player, stageStory);
            Game.SendEvent(GameEvent.Game_Stage_Update);
        }

        public void NextGameStage() => StageIndex++;
    }

    public class GameTag
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string AnimTrigger = "AnimTrigger";
        public const string AnimInt = "AnimInt";
    }
    public class GameEvent
    {
        public const string Stage_StageTime_Over = "Stage_StageTime_Over";// 关卡时间结束
        public const string GameItem_Interaction = "GameItem_Interaction";// 游戏物品交互
        public const string Stage_StageTime_Update = "Stage_StageTime_Update";
        public const string Game_Stage_Update = "Game_Stage_Update";// 游戏关卡更新

        //玩家
        public const string Player_Lantern_Update = "Player_Lantern_Update";// 玩家灯笼更新
        public const string Player_Panic_Pulse = "Player_Panic_Pulse";//恐慌心跳
        public const string Player_Panic_Finalize = "Player_Panic_Finalize";//恐慌心跳结束触发
        public const string Player_IsDeath = "Player_IsDeath";//玩家死亡
        public const string Player_Hp_Update = "Player_Hp_Update";//玩家血量更新
        public const string Player_Spell_Add = "Player_Spell_Add";//玩家法术添加

        //环境
        public const string Env_Lightning = "Env_Lightning";// 闪电
        public const string Env_Rain_Update= "Env_Rain_Update";// 下雨

        //游戏
        public const string Game_StateChanged = "Game_StateChanged";// 游戏状态改变
        public const string Game_PlayMode_Update = "Game_PlayMode_Update";//游戏模式更新

        //故事
        public const string Story_Lines_Send = "Story_Lines_Send"; // 游戏故事行发送
        public const string Story_Dialog_Send = "Story_Dialog_Send"; // 游戏故事对话发送
        public const string Story_Plot_Begin = "Story_Plot_Begin"; // 游戏情节开始
        public const string Story_End = "Story_End"; // 游戏故事结束

        //战斗
        public const string Battle_Spell_On_Player = "Battle_Spell_On_Player"; //战斗法术对玩家伤害
        public const string Battle_Spell_Update= "Battle_Spell_Update";//战斗法术释放
        public const string Battle_Skeleton_Death = "Battle_Skeleton_Death"; // 战斗单位死亡

        public const string Story_Npc_Update = "Story_Npc_Update";
    }
}