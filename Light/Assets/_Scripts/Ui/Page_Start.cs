using System;
using System.Threading.Tasks;
using GameData;
using GMVC.Core;
using GMVC.Views;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class Page_Start : UiBase
    {
        Text text_title { get; }
        Button btn_click { get; }

        public Page_Start(IView v, UnityAction onClickAction, bool display = true) : base(v, display)
        {
            text_title = v.Get<Text>("text_title");
            btn_click = v.Get<Button>("btn_click");
            btn_click.onClick.AddListener(async() =>
            {
                text_title.text = "加载中...";
                await Task.Delay(1000);
                onClickAction?.Invoke();
            });
            Game.RegEvent(GameEvent.Game_Start, b => Display(true));
        }
    }
}