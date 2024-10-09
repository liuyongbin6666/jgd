using GMVC.Core;
using GMVC.Views;
using UnityEngine.Events;
using UnityEngine.UI;

public class Page_Game : UiBase
{
    Text text_title { get; }
    Button btn_gameStart { get; }
    Button btn_gameOver { get; }

    public Page_Game(IView v, UnityAction onGameStartAction, UnityAction onGameOverAction, bool display = true) :
        base(v, display)
    {
        text_title = v.Get<Text>("text_title");
        btn_gameStart = v.Get<Button>("btn_gameStart");
        btn_gameOver = v.Get<Button>("btn_gameOver");
        btn_gameStart.onClick.AddListener(() =>
        {
            onGameStartAction?.Invoke();
        });
        btn_gameOver.onClick.AddListener(() =>
        {
            if (onGameOverAction != null) onGameOverAction.Invoke();
        });
        Game.RegEvent(GameEvent.Game_StateChanged, b =>
        {
            var state = Game.World.Status;
            Display(state == GameWorld.GameStates.Playing);
        });
    }
}