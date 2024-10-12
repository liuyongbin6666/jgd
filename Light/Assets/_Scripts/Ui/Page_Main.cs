using GMVC.Core;
using GMVC.Views;
using UnityEngine.Events;
using UnityEngine.UI;

public class Page_Main:UiBase
{
    private View_StartStage view_startStage { get; }
    public Page_Main(IView v,UnityAction toPageStage, bool display = true) : base(v, display)
    {
        view_startStage = new View_StartStage(v.Get<View>("view_startStage"), toPageStage);
    }

    class View_StartStage:UiBase
    {
        Button btn_startStage { get; }
        public View_StartStage(IView v,UnityAction toPageStage, bool display = true) : base(v, display)
        {
            btn_startStage = v.Get<Button>("btn_startStage");
            btn_startStage.onClick.AddListener(() =>
            {
                toPageStage.Invoke();
                StartStage();
            });
        }

        void StartStage()
        {
            Game.GetController<GameController>().Game_StartStage();
        }
    }
}