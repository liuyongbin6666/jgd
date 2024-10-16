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

    public void InitStage(PlayableUnit player, StageIndex stageIndex, StageTime stageTime)
    {
        Stage = new GameStage(player,stageIndex,stageTime);
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
    public const string Player_Lantern_Update = "Player_Lantern_Update";// 玩家灯笼更新
    public const string Player_Panic_Pulse = "Player_Panic_Pulse";//恐慌心跳
    public const string Player_Panic_Finalize = "Player_Panic_Finalize";//恐慌心跳结束触发
    public const string Env_Lightning = "Env_Lightning";// 闪电

    public const string Game_StateChanged = "Game_StateChanged";// 游戏状态改变
    public const string Game_PlayMode_Update = "Game_PlayMode_Update";//游戏模式更新

    public const string Story_Npc_Update = "Story_Npc_Update";
    public const string Story_Show = "Story_Show";
}