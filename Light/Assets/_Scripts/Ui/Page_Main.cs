using Controller;
using GameData;
using GMVC.Core;
using GMVC.Views;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class Page_Main:UiBase
    {
        View_StartStage view_startStage { get; }
        GameController GameController => Game.GetController<GameController>();
        public Page_Main(IView v, bool display = true) : base(v, display)
        {
            view_startStage = new View_StartStage(v.Get<View>("view_startStage"),GameController.Game_StartStage);
            Game.RegEvent(GameEvent.Game_StateChanged, _ =>
            {
                var isMain = Game.World.Status == GameWorld.GameStates.Start;
                Display(isMain);
            });
        }

        class View_StartStage:UiBase
        {
            Button btn_startStage { get; }
            public View_StartStage(IView v,UnityAction onBtnClick,bool display = true) : base(v, display)
            {
                btn_startStage = v.Get<Button>("btn_startStage");
                btn_startStage.onClick.AddListener(onBtnClick);
            }
        }
    }
}