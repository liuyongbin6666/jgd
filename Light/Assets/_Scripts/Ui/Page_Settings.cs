using System;
using Components;
using Controller;
using GameData;
using GMVC.Core;
using GMVC.Views;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ui
{
    public class Page_Settings : UiBase
    {
        AudioComponent audio;
        AudioMixer audioMixer => audio.audioMixer;
        const string BGMVolumeKey = "BGMVolumeKey";
        const string SFXVolumeKey = "SFXVolumeKey";
        Button btn_settings { get; }
        View_Settings view_settings { get; }
        GameController GameController => Game.GetController<GameController>();

        public Page_Settings(IView v, bool display = true) : base(v, display)
        {
            audio = v.Get<AudioComponent>("音效");
            btn_settings = v.Get<Button>("btn_settings");
            view_settings = new View_Settings(v.Get<View>("view_settings"), audioMixer, ()=>
                {
                    GameController.Game_Save();
                    GameController.Game_Resume();
                },
                GameController.Game_Resume);
            btn_settings.onClick.AddListener(() =>
            {
                view_settings.LoadVolumeSettings();
                view_settings.Show();
                GameController.Game_Pause();
            });
            Game.RegEvent(GameEvent.Game_Playing, _ => Show());
            Game.RegEvent(GameEvent.Game_Stage_Start , _ => Hide());
            Game.RegEvent(GameEvent.Game_End, _ => Hide());
            Game.RegEvent(GameEvent.Stage_StageTime_Update, _ => view_settings.UpdateTime());
        }

        class View_Settings : UiBase
        {
            string bgmChannel = "BGM"; // BGM频道参数名
            string sfxChannel = "SFX"; // SFX频道参数名
            float musicVolume;
            float soundEffectVolume;
            Button btn_close { get; }
            Button btn_save { get; }
            Slider slider_bgm { get; }
            Slider slider_sfx { get; }
            View_Time view_time { get; }
            AudioMixer audioMixer; // 音频混合器


            public View_Settings(IView v, AudioMixer mixer, UnityAction onSaveAction, UnityAction onCloseAction) :
                base(v, false)
            {
                audioMixer = mixer;
                btn_close = v.Get<Button>("btn_close");
                btn_save = v.Get<Button>("btn_save");
                btn_save.onClick.AddListener(() =>
                {
                    SetMusicVolume(musicVolume);
                    SetSoundEffectVolume(soundEffectVolume);
                    onSaveAction?.Invoke();
                    Hide();
                });
                slider_bgm = v.Get<Slider>("slider_bgm");
                slider_bgm.onValueChanged.AddListener(f =>
                {
                    musicVolume = f;
                    ApplyVolumeSettings();
                });
                slider_sfx = v.Get<Slider>("slider_sfx");
                slider_sfx.onValueChanged.AddListener(f =>
                {
                    soundEffectVolume = f;
                    ApplyVolumeSettings();
                });
                view_time = new View_Time(v.Get<View>("view_time"));
                btn_close.onClick.AddListener(() =>
                {
                    SetMusicVolume(musicVolume);
                    SetSoundEffectVolume(soundEffectVolume);
                    onCloseAction?.Invoke();
                    Hide();
                });
            }

            public void LoadVolumeSettings()
            {
                var bgmVol = 1f;
                var sfxVol = 1f;
                audioMixer.GetFloat(bgmChannel, out bgmVol);
                audioMixer.GetFloat(sfxChannel, out sfxVol);
                musicVolume = PlayerPrefs.GetFloat(BGMVolumeKey, bgmVol);
                soundEffectVolume = PlayerPrefs.GetFloat(SFXVolumeKey, sfxVol);
                // 设置音量
                ApplyVolumeSettings();
            }

            void ApplyVolumeSettings()
            {
                // 使用分贝值来设置 Mixer 中的音量 (-80 dB 静音, 0 dB 最大音量)
                audioMixer.SetFloat(bgmChannel, Mathf.Log10(musicVolume) * 20);
                audioMixer.SetFloat(sfxChannel, Mathf.Log10(soundEffectVolume) * 20);
            }

            // 设置并保存音乐音量
            void SetMusicVolume(float volume)
            {
                musicVolume = Mathf.Clamp(volume, 0.0001f, 1f); // 避免使用 0
                audioMixer.SetFloat(bgmChannel, Mathf.Log10(musicVolume) * 20);
                PlayerPrefs.SetFloat(BGMVolumeKey, musicVolume);
                PlayerPrefs.Save(); // 保存设置
            }

            // 设置并保存音效音量
            void SetSoundEffectVolume(float volume)
            {
                soundEffectVolume = Mathf.Clamp(volume, 0.0001f, 1f); // 避免使用 0
                audioMixer.SetFloat(sfxChannel, Mathf.Log10(soundEffectVolume) * 20);
                PlayerPrefs.SetFloat(SFXVolumeKey, soundEffectVolume);
                PlayerPrefs.Save(); // 保存设置
            }

            public void UpdateTime()
            {
                var secs = Game.World.Stage.Story.Seconds;
                var ts = TimeSpan.FromSeconds(secs);
                view_time.Set((int)ts.TotalMinutes, ts.Seconds);
            }
        }
    }
}