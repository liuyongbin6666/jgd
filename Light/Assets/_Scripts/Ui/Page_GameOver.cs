using GameData;
using GMVC.Core;
using GMVC.Views;
using UnityEngine.UI;

namespace Ui
{
    public class Page_GameOver : UiBase
    {
        Text text_title { get; }
        Button btn_restart { get; }

        public Page_GameOver(IView v,bool display = true) : base(v, display)
        {
            text_title = v.Get<Text>("text_title");
            Game.RegEvent(GameEvent.Stage_Lose, b => Display(true));
        }
    }
}