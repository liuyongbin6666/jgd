using GMVC.Core;
using GMVC.Utls;
using GMVC.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Page_Game : UiBase
{
    View_PlayerInfo view_playerInfo { get; }
    View_Joystick view_joystick { get; }
    PlayableController PlayableController => Game.GetController<PlayableController>();
    public Page_Game(IView v) :
        base(v)
    {
        view_playerInfo = new View_PlayerInfo(v.Get<View>("view_playerInfo"));
        view_joystick = new View_Joystick(v.Get<View>("view_joystick"), PlayableController.Move);
        Game.RegEvent(GameEvent.Game_StateChanged, b =>
        {
            var state = Game.World.Status;
            var playingMode = state == GameWorld.GameStates.Playing;
            Display(playingMode);
            view_playerInfo.UpdateLantern(Game.World.Stage.Player.Lantern);
            view_joystick.SetActive(playingMode);
        });
        Game.RegEvent(GameEvent.Player_Lantern_Update,
                      _ => view_playerInfo.UpdateLantern(Game.World.Stage.Player.Lantern));
    }

    class View_PlayerInfo : UiBase
    {
        Element_TextValue element_textValue_lantern { get; }
        public View_PlayerInfo(IView v) : base(v)
        {
            element_textValue_lantern = new Element_TextValue(v.Get<View>("element_textValue_lantern"), "虫灯");
            element_textValue_lantern.SetValue(0);
        }
        public void UpdateLantern(int value) => element_textValue_lantern.SetValue(value);
        class Element_TextValue : UiBase
        {
            Text text_title { get; }
            Text text_value { get; }
            public Element_TextValue(IView v,string title) : base(v)
            {
                text_title = v.Get<Text>("text_title");
                text_value = v.Get<Text>("text_value");
                text_title.text = title;
            }
            public void SetValue(object value) => text_value.text = value.ToString();
        }
    }

    class View_Joystick : UiBase
    {
        JoyStick joyStick { get; }
        public View_Joystick(IView v,UnityAction<Vector3> onMoveAction,bool display = true) : base(v, display)
        {
            joyStick = v.Get<JoyStick>("JoyStick");
            joyStick.Init();
            joyStick.OnMoveEvent.RemoveAllListeners();
            joyStick.OnMoveEvent.AddListener(onMoveAction);
        }
        public void SetActive(bool active) => joyStick.Display(active);
    }
}