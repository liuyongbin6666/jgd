using GMVC.Core;

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

    public void InitStage(PlayableUnit player, StageIndex stageIndex, StageStory stageStory)
    {
        Stage = new GameStage(player,stageIndex,stageStory);
        SetState(GameStates.Start);
    }
    public void StartStage()
    {
        Stage.Stage_Start();
        SetState(GameStates.Playing);
    }
    public void End()
    {
        SetState(GameStates.End);
    }

    void SetState(GameStates state)
    {
        Status = state;
        Game.SendEvent(GameEvent.Game_StateChanged, state);
    }
}

public class GameTag
{
    public const string Firefly = "Firefly";
    public const string GameItem = "GameItem";
    public const string Player = "Player";
}
public class GameEvent
{
    public const string GameItem_Interaction = "GameItem_Interaction";// 游戏物品交互
    public const string Stage_StageTime_Update = "Stage_StageTime_Update";

    //玩家
    public const string Player_Lantern_Update = "Player_Lantern_Update";// 玩家灯笼更新
    public const string Player_Panic_Pulse = "Player_Panic_Pulse";//恐慌心跳
    public const string Player_Panic_Finalize = "Player_Panic_Finalize";//恐慌心跳结束触发
    public const string Player_IsDeath = "Player_IsDeath";//玩家死亡
    public const string Player_Hp_Update = "Player_Hp_Update";//玩家血量更新

    //环境
    public const string Env_Lightning = "Env_Lightning";// 闪电
    public const string Env_Rain_Update= "Env_Rain_Update";// 下雨

    //游戏
    public const string Game_StateChanged = "Game_StateChanged";// 游戏状态改变
    public const string Game_PlayMode_Update = "Game_PlayMode_Update";//游戏模式更新

    //故事
    public const string Story_Lines_Send = "Story_Lines_Send"; // 游戏故事行发送
    public const string Story_Dialog_Send = "Story_Dialog_Send"; // 游戏故事对话发送

    public const string Story_Npc_Update = "Story_Npc_Update";
}