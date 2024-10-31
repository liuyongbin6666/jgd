using Controller;
using GameData;
using GMVC.Core;
using GMVC.Utls;
using GMVC.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class Page_Main:UiBase
    {
        PlayerSave save;
        View_StartStage view_startStage { get; }
        GameController GameController => Game.GetController<GameController>();
        public Page_Main(IView v, bool display = true) : base(v, display)
        {
            view_startStage = new View_StartStage(v.Get<View>("view_startStage"), OnStartBtnClick,OnLoadStage);

            var json = PlayerPrefs.GetString(GameTag.PlayerSaveString);
            save = Json.Deserialize<PlayerSave>(json);
            view_startStage.DisplayLoadBtn(save != null);

            Game.RegEvent(GameEvent.Game_Start, _ => Display(true));
        }

        void OnLoadStage()
        {
            GameController.Game_LoadStage();
            Display(false);
        }

        void OnStartBtnClick()
        {
            var hasSaveFile = save != null;
            if (hasSaveFile)
                view_startStage.SetConfirmation(OnLoadStage);
            else
                GameController.Game_StartNewStage();
            Display(hasSaveFile);
        }

        class View_StartStage:UiBase
        {
            Button btn_startStage { get; }
            Button btn_loadStage { get; }
            View_Confirmation view_confirmation { get; }
            public View_StartStage(IView v,UnityAction onStartBtnClick,UnityAction onLoadStage) : base(v)
            {
                btn_startStage = v.Get<Button>("btn_startStage");
                btn_startStage.onClick.AddListener(onStartBtnClick);
                btn_loadStage = v.Get<Button>("btn_loadStage");
                btn_loadStage.onClick.AddListener(onLoadStage);
                view_confirmation = new View_Confirmation(v.Get<View>("view_confirmation"));
            }

            public void SetConfirmation(UnityAction onConfirmAction) =>
                view_confirmation.SetConfirm(onConfirmAction);
            public void DisplayLoadBtn(bool display) => btn_loadStage.Display(display);

            class View_Confirmation : UiBase
            {
                Button btn_confirm { get; }
                Button btn_cancel { get; }
                public View_Confirmation(IView v) : base(v, false)
                {
                    btn_confirm = v.Get<Button>("btn_confirm");
                    btn_cancel = v.Get<Button>("btn_cancel");
                    btn_cancel.onClick.AddListener(() => Display(false));
                }

                public void SetConfirm(UnityAction confirmAction)
                {
                    btn_confirm.onClick.RemoveAllListeners();
                    btn_confirm.onClick.AddListener(() =>
                    {
                        confirmAction();
                        Hide();
                    });
                    Show();
                }
            }
        }
    }
}