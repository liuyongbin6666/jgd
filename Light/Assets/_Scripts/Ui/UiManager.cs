using GMVC.Core;
using GMVC.Views;

public class UiManager : UiManagerBase
{
    public View page_start;
    public View page_game;
    public View page_gameOver;
    Page_Start PageStart { get; set; }
    Page_Game PageGame { get; set; }
    Page_GameOver PageGameOver { get; set; }
    GameController GameController => Game.GetController<GameController>();
    public override void Init()
    {
        //PageStart = new Page_Start(page_start, GameController.Game_Playing);
        //PageGame = new Page_Game(page_game, GameController.Game_Start, GameController.Game_End);
        //PageGameOver = new Page_GameOver(page_gameOver);
    }
}