using GMVC.Core;
using GMVC.Views;
using UnityEngine;
using UnityEngine.UI;

public class Page_Stage : UiBase
{
    View_Top view_top { get; }
    view_Npc view_npc { get; }

    public Page_Stage(IView v) :
        base(v)
    {
        view_top = new View_Top(v.Get<View>("view_top"));
        Game.RegEvent(GameEvent.Game_StateChanged, b =>
        {
            var state = Game.World.Status;
            Display(state == GameWorld.GameStates.Playing);
            view_top.UpdateLantern(Game.World.Stage.Player.Lantern);
        });
        Game.RegEvent(GameEvent.Player_Lantern_Update,
                      _ => view_top.UpdateLantern(Game.World.Stage.Player.Lantern));

        Game.RegEvent(GameEvent.Stage_StageTime_Update,_=>view_top.UpdateStageTime(Game.World.Stage.Time.RemainSeconds));

        view_npc = new view_Npc(v.Get<View>("view_npc"));
        Game.RegEvent(GameEvent.Story_Npc_Update, bag => view_npc.SetNpcTalk(bag.Get<string>(0)));
    }

    class View_Top : UiBase
    {
        Element_TextValue element_textValue_lantern { get; }
        Text text_minutes { get; }
        Text text_seconds { get; }
        Text text_f { get; }

        public View_Top(IView v) : base(v)
        {
            element_textValue_lantern = new Element_TextValue(v.Get<View>("element_textValue_lantern"));
            element_textValue_lantern.SetValue(0);

            text_minutes = v.Get<Text>("text_minutes");
            text_seconds = v.Get<Text>("text_seconds");
            text_f = v.Get<Text>("text_f");
        }

        public void UpdateStageTime(int totalSeconds)
        {
            var minutes = totalSeconds / 60; // 计算分钟数
            text_minutes.text = minutes.ToString();
            var seconds = totalSeconds % 60; // 计算剩余的秒数
            text_seconds.text = seconds.ToString();

            if (totalSeconds<=30)
            {
                text_minutes.color=Color.red;
                text_seconds.color=Color.red;
                text_f.color=Color.red;
            }
            else
            {
                text_minutes.color=Color.yellow;
                text_seconds.color=Color.yellow;
                text_f.color = Color.yellow;
            }
        }
        public void UpdateLantern(int value) => element_textValue_lantern.SetValue(value);
        class Element_TextValue : UiBase
        {
            Text text_value { get; }
            public Element_TextValue(IView v) : base(v)
            {
                text_value = v.Get<Text>("text_value");
            }
            public void SetValue(object value) => text_value.text = value.ToString();
        }
    }

    class view_Npc:UiBase
    {
        Text text_npcTalk { get; }
        public view_Npc(IView v, bool display = true) : base(v, display)
        {
            text_npcTalk = v.Get<Text>("text_npcTalk");
        }

        public void SetNpcTalk(string str)
        {
            text_npcTalk.text = str;
        }
    }
}