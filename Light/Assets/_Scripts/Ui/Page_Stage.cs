using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using DG.Tweening;
using GameData;
using GMVC.Core;
using GMVC.Utls;
using GMVC.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class Page_Stage : UiBase
    {
        View_Top view_top { get; }
        View_Npc view_npc { get; }
        View_StoryPlayer view_storyPlayer { get; }
        View_Win view_win { get; }
        View_Defeat view_defeat { get; }
        View_Joystick view_joystick { get; }
        View_lantern view_latern { get; }
        View_Effect view_effect { get; }
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

            view_win = new View_Win(v.Get<View>("view_win"),GameController.Game_NextStage);//todo 下一关事件
            view_defeat = new View_Defeat(v.Get<View>("view_defeat"),GameController.Game_End);//todo 返回page main事件
            view_latern = new View_lantern(v.Get<View>("view_lantern"), PlayableController.ChargeSpell);
            view_effect = new View_Effect(v.Get<View>("view_effect"));

            /**********事件注册**********/
            Game.RegEvent(GameEvent.Game_StateChanged, b =>
            {
                view_joystick.SetActive(IsPlaying && IsExploring);
                Display(IsPlaying);
                if (!IsPlaying) return;
                view_top.UpdateLantern(Stage.Player.Lantern);
            });
            Game.RegEvent(GameEvent.Game_PlayMode_Update, _ => view_joystick.SetActive(IsExploring));
            Game.RegEvent(GameEvent.Player_Lantern_Update, _ =>
            {
                view_top.UpdateLantern(Stage.Player.Lantern);
                view_latern.SetLantern((float)Stage.Player.Firefly.ValueMaxRatio);
                view_latern.SetHp((float)Stage.Player.Hp.ValueMaxRatio);
            });
            Game.RegEvent(GameEvent.Player_Hp_Update,_=>view_latern.SetHp((float)Stage.Player.Hp.ValueFixRatio));
            Game.RegEvent(GameEvent.Player_Panic_Finalize, _ =>
            {
                view_latern.SetPanic(0);
                view_latern.SetLantern(0);
                view_defeat.Show();
            });
            Game.RegEvent(GameEvent.Player_Panic_Pulse, b =>
            {
                var remain = b.Get<int>(0);
                var max = b.Get<int>(1);
                view_latern.SetPanic(1f * remain / max);
                view_effect.PlayPanic();
            });
            Game.RegEvent(GameEvent.Player_Spell_Add, _ => view_latern.SetSpell(Stage.Player.Spells));
            Game.RegEvent(GameEvent.Battle_Spell_Finish, b => view_latern.SpellRemoveAt(b.Get<int>(0)));
            Game.RegEvent(GameEvent.Battle_Spell_Update, b =>
            {
                var index = b.Get<int>(0);
                var remain = b.Get<int>(1);
                var max = b.Get<int>(2);
                view_latern.SpellUpdate(index, 1f * remain / max);
                view_latern.SetSelected(Stage.Player.SelectedSpellIndex);
            });
            Game.RegEvent(GameEvent.Stage_StageTime_Update, _ => view_top.UpdateStageTime(Stage.Story.RemainSeconds));
            Game.RegEvent(GameEvent.Story_Lines_Send, b => view_storyPlayer.ShowStory(Stage.Story.StoryLines));
            Game.RegEvent(GameEvent.Story_Dialog_Send, b => view_npc.SetNpcTalk(Stage.Story.DialogLines.ToList()));
        }

        class View_Effect : UiBase
        {
            Image image_panic { get; }

            public View_Effect(IView v) : base(v)
            {
                image_panic = v.Get<Image>("image_panic");
                image_panic.Display(false);
            }
            Coroutine Panic;

            public void PlayPanic()
            {
                if (Panic != null) return;
                Panic = StartCoroutine(PanicRoutine());

                IEnumerator PanicRoutine()
                {
                    image_panic.Display(true);
                    var color = image_panic.color;
                    yield return DOTween.Sequence()
                        .Append(image_panic.DOFade(0, 0.5f).SetEase(Ease.InBounce))
                        .AppendCallback(() =>
                        {
                            image_panic.Display(false);
                            image_panic.color = color;
                        });
                    Panic = null;
                }
            }

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
        class View_Win:UiBase
        {
            Button btn_next { get; }
            public View_Win(IView v,UnityAction onNextAction) : base(v, false)
            {
                btn_next = v.Get<Button>("btn_next");
                btn_next.onClick.AddListener(onNextAction);
            }
        }
        class View_Defeat:UiBase
        {
            Button btn_return { get; }
            public View_Defeat(IView v,UnityAction onReturnAction) : base(v, false)
            {
                btn_return = v.Get<Button>("btn_return");
                btn_return.onClick.AddListener(onReturnAction);
            }
        }
        class View_Joystick : UiBase
        {
            JoyStick.JoyStick joyStick { get; }
            public View_Joystick(IView v, UnityAction<Vector3> onMoveAction) : base(v)
            {
                joyStick = v.Get<JoyStick.JoyStick>("Joystick");
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
        class View_lantern:UiBase
        {
            View_Condition view_condition { get; }
            ListView_Trans<Prefab_Spell> SpellList { get; }
            event UnityAction<int> OnSpellAction;
            public View_lantern(IView v,UnityAction<int> onSpellAction ,bool display = true) : base(v, display)
            {
                OnSpellAction = onSpellAction;
                view_condition = new View_Condition(v.Get<View>("view_condition"));
                SpellList = new ListView_Trans<Prefab_Spell>(v, "prefab_spell", "tran_layout");
            }
            public void SetLantern(float value) => view_condition.SetLantern(value);
            public void SetHp(float value) => view_condition.SetHp(value);
            public void SetPanic(float value) => view_condition.SetPanic(value);
            public void SpellUpdate(int index, float remain) => SpellList.List[index].SetValue(remain);
            public void SpellRemoveAt(int index)
            {
                var ui = SpellList.List[index];
                SpellList.Remove(ui);
                ui.Destroy();
            }
            public void SetSelected(int index)
            {
                for (var i = 0; i < SpellList.List.Count; i++)
                {
                    var ui = SpellList.List[i];
                    ui.SetSelected(index == i);
                }
            }
            public void SetSpell(List<Spell> spells)
            {
                SpellList.ClearList(u=>u.Destroy());
                for (int i = 0; i < spells.Count; i++)
                {
                    var spell = spells[i];
                    var index = i;
                    var ui = SpellList.Instance(v => new Prefab_Spell(v, () => OnSpellAction?.Invoke(index)));
                    ui.SetImage(Game.Config.SpellSo.GetSprite(spell.SpellName));
                    ui.SetValue(1);
                }
            }
            class Prefab_Spell : UiBase
            {
                Slider slider_value { get; }
                Button btn_click { get; }
                Image img_bg { get; }
                Image img_selected { get; }
                public Prefab_Spell(IView v,UnityAction onClickAction ,bool display = true) : base(v, display)
                {
                    slider_value = v.Get<Slider>("slider_value");
                    btn_click = v.Get<Button>("btn_click");
                    btn_click.onClick.AddListener(onClickAction);
                    img_bg = v.Get<Image>("img_bg");
                    img_selected = v.Get<Image>("img_selected");
                }
                public void SetImage(Sprite sprite) => img_bg.sprite = sprite;
                public void SetValue(float value) => slider_value.value = 1 - value;
                public void SetSelected(bool selected) => img_selected.Display(selected);
            }
            class View_Condition : UiBase
            {
                Slider slider_lantern { get; }
                View_Hp view_hp { get; }
                public View_Condition(IView v, bool display = true) : base(v, display)
                {
                    slider_lantern = v.Get<Slider>("slider_lantern");
                    view_hp = new View_Hp(v.Get<View>("view_hp"));
                }
                public void SetLantern(float value) => slider_lantern.value = value;
                public void SetHp(float value)
                {
                    view_hp.SwitchPanic(false);
                    view_hp.SetHp(value);
                }
                public void SetPanic(float value)
                {
                    view_hp.SwitchPanic(true);
                    view_hp.SetPanic(value);
                }
                class View_Hp : UiBase
                {
                    Slider slider_panic { get; }
                    Slider slider_hp { get; }
                    public View_Hp(IView v, bool display = true) : base(v, display)
                    {
                        slider_panic = v.Get<Slider>("slider_panic");
                        slider_hp = v.Get<Slider>("slider_hp");
                    }
                    public void SetHp(float value) => slider_hp.value = value;
                    public void SetPanic(float value) => slider_panic.value = value;
                    public void SwitchPanic(bool isPanic)
                    {
                        slider_hp.Display(!isPanic);
                    }
                }
            }
        }
    }
}