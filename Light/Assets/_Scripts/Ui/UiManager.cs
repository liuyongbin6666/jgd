using Controller;
using GMVC.Core;
using GMVC.Utls;
using GMVC.Views;

namespace Ui
{
    public class UiManager : UiManagerBase
    {
        public View page_start;
        public View page_main;
        public View page_stage;
        public View page_gameOver;
        Page_Start PageStart { get; set; }
        Page_Main PageMain { get; set; }
        Page_Stage PageStage { get; set; }
        Page_GameOver PageGameOver { get; set; }
        GameController GameController => Game.GetController<GameController>();
        public override void Init()
        {
            //PageStart = new Page_Start(page_start, GameController.Game_Playing);
            PageMain = new Page_Main(page_main);
            PageStage = new Page_Stage(page_stage);
            //PageGameOver = new Page_GameOver(page_gameOver);
            this.Display(true);
        }
    }
}