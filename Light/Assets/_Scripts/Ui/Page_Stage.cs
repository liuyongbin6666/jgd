using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GMVC.Core;
using GMVC.Utls;
using GMVC.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class Page_Stage : UiBase
{
    View_Top view_top { get; }
    View_Npc view_npc { get; }
    View_StoryPlayer view_storyPlayer { get; }
    View_Joystick view_joystick { get; }
    PlayableController PlayableController => Game.GetController<PlayableController>();
    GameController GameController => Game.GetController<GameController>();
    GameWorld World => Game.World;
    GameStage Stage => Game.World.Stage;
    GameWorld.GameStates State => World.Status;
    bool IsPlaying => State == GameWorld.GameStates.Playing;
    bool IsExploring => Stage is { Mode: GameStage.PlayModes.Explore };
    public Page_Stage(IView v) :
        base(v)
    {
        view_top = new View_Top(v.Get<View>("view_top"));
        view_joystick = new View_Joystick(v.Get<View>("view_joystick"), PlayableController.Move);
        view_npc = new View_Npc(v.Get<View>("view_npc"));
        view_storyPlayer = new View_StoryPlayer(v.Get<View>("view_storyPlayer"), () =>
        {
            //GameController.SwitchPlayMode(GameStage.PlayModes.Explore);
            view_storyPlayer.Hide();
        });

        /**********事件注册**********/

        Game.RegEvent(GameEvent.Game_StateChanged, b =>
        {
            view_joystick.SetActive(IsPlaying && IsExploring);
            Display(IsPlaying);
            if (!IsPlaying) return;
            view_top.UpdateLantern(Stage.Player.Lantern);
        });
        Game.RegEvent(GameEvent.Game_PlayMode_Update, _ => view_joystick.SetActive(IsExploring));
        Game.RegEvent(GameEvent.Player_Lantern_Update, _ => view_top.UpdateLantern(Stage.Player.Lantern));
        Game.RegEvent(GameEvent.Stage_StageTime_Update, _ => view_top.UpdateStageTime(Stage.Story.RemainSeconds));
        Game.RegEvent(GameEvent.Story_Lines_Send, b => view_storyPlayer.ShowStory(Stage.Story.StoryLines));
        Game.RegEvent(GameEvent.Story_Dialog_Send, b => view_npc.SetNpcTalk(Stage.Story.DialogLines.ToList()));
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

            if (totalSeconds <= 30)
            {
                text_minutes.color = Color.red;
                text_seconds.color = Color.red;
                text_f.color = Color.red;
            }
            else
            {
                text_minutes.color = Color.yellow;
                text_seconds.color = Color.yellow;
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

    class View_Npc : UiBase
    {
        Text text_npcTalk { get; }
        public View_Npc(IView v, bool display = true) : base(v, display)
        {
            text_npcTalk = v.Get<Text>("text_npcTalk");
        }

        public void SetNpcTalk(List<string> lines)
        {
            StopAllCoroutines();
            StartCoroutine(TalkRoutine());
            return;

            IEnumerator TalkRoutine()
            {
                while (lines.Count > 0)
                {
                    var line = lines[0];
                    lines.RemoveAt(0);
                    text_npcTalk.text = line;
                    yield return new WaitForSeconds(1f);
                }
            }
        }

    }

    class View_StoryPlayer : UiBase
    {
        GameObject obj_textList { get; }
        Button btn_skip { get; }

        public View_StoryPlayer(IView v,UnityAction onSkipAction) : base(v, false)
        {
            obj_textList = v.Get("obj_textList");
            btn_skip = v.Get<Button>("btn_skip");
            btn_skip.onClick.AddListener(onSkipAction);
        }

        public void ShowStory(string[] story)
        {
            Show();
            var t = obj_textList.transform;
            for (int i = 0; i < t.childCount; i++)
                t.GetChild(i).gameObject.SetActive(false);
            ShowText(0, story);
        }

        void ShowText(int j, string[] story)
        {
            float showSpeed = 1f;
            var t = obj_textList.transform;

            if (j < story.Length)
            {
                string str = story[j];
                t.GetChild(j).GetComponent<Text>().text = "";
                t.GetChild(j).gameObject.SetActive(true);
                t.GetChild(j).GetComponent<Text>().text = str;
                t.GetChild(j).GetComponent<Text>().DOFade(1f, showSpeed).SetEase(Ease.Unset)
                    .OnComplete(
                        delegate ()
                        {
                            t.GetChild(j).GetComponent<Text>().DOFade(1, showSpeed * 2)
                                .SetEase(Ease.Unset);
                            ShowText(j + 1, story);
                        });
            }
        }
    }


    class View_Joystick : UiBase
    {
        JoyStick joyStick { get; }
        public View_Joystick(IView v, UnityAction<Vector3> onMoveAction) : base(v)
        {
            joyStick = v.Get<JoyStick>("Joystick");
            joyStick.Init();
            joyStick.OnMoveEvent.RemoveAllListeners();
            joyStick.OnMoveEvent.AddListener(onMoveAction);
        }

        public void SetActive(bool active)
        {
            joyStick.ResetJoystick();
            joyStick.Display(active);
        }
    }
}