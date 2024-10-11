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

    public void Start(PlayableUnit player, int gameTimeSecs = 300)
    {
        Stage = new GameStage(player, gameTimeSecs);
        SetState(GameStates.Start);
    }
    public void Playing()
    {
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
}
public class GameEvent
{
    public const string Game_StateChanged = "Game_StateChanged";// 游戏状态改变
    public const string Player_Lantern_Update = "Player_Lantern_Update";// 玩家灯笼更新
    public const string Player_Panic_Pulse = "Player_Panic_Pulse";//恐慌心跳
    public const string Player_Panic_Finalize = "Player_Panic_Finalize";//恐慌心跳结束触发
}