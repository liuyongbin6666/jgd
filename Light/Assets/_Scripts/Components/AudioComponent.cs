using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class AudioComponent : AudioManager
{
    const string PlayingNone = "未播放";
    public AudioCfgSo BgmSo;
    public AudioCfgSo SfxSo;
    [SerializeField] AudioMixer audioMixer;
    [HideInEditorMode,ValueDropdown(nameof(GetBGMNames)),OnValueChanged(nameof(SetBGMPlaying))] public string BGM = PlayingNone;
    [HideInEditorMode,ValueDropdown(nameof(GetSFXNames)),OnValueChanged(nameof(SetSFXPlaying))] public string SFX = PlayingNone;
    [SerializeField] List<SFXBinding> sfxBindings;
    Dictionary<string, string> _gameEventMap;
    Dictionary<string,string> GameEventMap
    {
        get
        {
            if (_gameEventMap == null)
                _gameEventMap = new Dictionary<string, string>
                {
                    { "闪电", GameEvent.Env_Lightning },
                    { "恐慌", GameEvent.Player_Panic_Pulse },
                };
            return _gameEventMap;
        }
    }
    Coroutine PlayingRoutine { get; set; }
    public void Init()
    {
        foreach (var sfxBinding in sfxBindings)
            Game.RegEvent(GameEventMap[sfxBinding.gameEvent],
                _ =>
                {
                    Play(Types.SFX, SfxSo.GetClip(sfxBinding.SFX));
                });
        Game.RegEvent(GameEvent.Game_StateChanged, _ =>
        {
            var isPlaying = Game.World.Status == GameWorld.GameStates.Playing;
            if (isPlaying)
                PlayingRoutine = Game.StartCoService(PlayerStep);
            else
                Game.StopCoService(PlayingRoutine);
        });
    }


    IEnumerator PlayerStep(GameWorld world)
    {
        while (true)
        {
            while (world.Stage.Player.IsMoving)
            {
                SetSFXPlaying("steps");
                yield return new WaitForSeconds(1.5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    public IEnumerable<string> GetGameEvents() => GameEventMap.Keys;
    string[] GetBGMNames() => new[] { PlayingNone }.Concat(BgmSo.GetNames()).ToArray();
    string[] GetSFXNames() => new[] { PlayingNone }.Concat(SfxSo.GetNames()).ToArray();
    public void SetBGMPlaying(string bgmName)
    {
        if (bgmName == PlayingNone)
        {
            Stop(Types.BGM);
            return;
        }
        Play(Types.BGM, BgmSo.GetClip(bgmName));
    }
    public void SetSFXPlaying(string sfxName)
    {
        if (sfxName == PlayingNone)
        {
            Stop(Types.SFX);
            return;
        }
        Play(Types.SFX, SfxSo.GetClip(sfxName), () => SFX = PlayingNone);
    }

    [Serializable] class SFXBinding
    {
        [ValueDropdown("@((AudioComponent)$property.Tree.WeakTargets[0]).GetGameEvents()")] public string gameEvent;
        [ValueDropdown("@((AudioComponent)$property.Tree.WeakTargets[0]).GetSFXNames()")] public string SFX = PlayingNone;
    }
}