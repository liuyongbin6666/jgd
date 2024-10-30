using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData;
using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Components
{
    public class AudioComponent : AudioManager
    {
        const string PlayingNone = "未播放";
        public AudioCfgSo BgmSo;
        public AudioCfgSo SfxSo;
        [SerializeField] AudioMixer audioMixer;
        [HideInEditorMode,ValueDropdown(nameof(GetBGMNames)),OnValueChanged(nameof(SetBGMPlaying))] public string BGM = PlayingNone;
        [HideInEditorMode,ValueDropdown(nameof(GetSFXNames)),OnValueChanged(nameof(SetSFXPlaying))] public string SFX = PlayingNone;
        [SerializeField] List<SFXBinding> sfxBindings;
        [SerializeField] List<BGMBinding> bgmBindings;
        Dictionary<string, string> _gameEventMap;

        Dictionary<string, string> GameEventMap =>
            _gameEventMap ??= new Dictionary<string, string>
            {
                { "闪电", GameEvent.Env_Lightning },
                { "恐慌", GameEvent.Player_Panic_Pulse },
                { "Boss战斗", GameEvent.Story_Boss_Battle },
                { "Boss死亡", GameEvent.Story_Boss_Death },
                { "道具交互", GameEvent.GameItem_Interaction },
                { "灵魂交互", GameEvent.Story_Soul_Interactive},
                { "情节文本", GameEvent.Story_Lines_Send},
                { "法术释放", GameEvent.Spell_Cast },
                { "法术补充", GameEvent.Spell_Charge },
                { "敌人死亡", GameEvent.Battle_Skeleton_Death },
                { "子弹爆炸", GameEvent.Battle_Bullet_Explode },
                { "关卡失败", GameEvent.Stage_Lose},
                { "关卡完结", GameEvent.Stage_Complete},
                { "游戏开始", GameEvent.Game_Start },
                { "故事开启", GameEvent.Story_Begin },
                { "故事结束", GameEvent.Story_End },
            };

        Coroutine PlayingRoutine { get; set; }
        public void Init()
        {
            foreach (var sfxBinding in sfxBindings)
                Game.RegEvent(GameEventMap[sfxBinding.gameEvent],
                              _ =>
                              {
                                  Play(Types.SFX, SfxSo.GetClip(sfxBinding.SFX));
                              });
            foreach (var bgmBinding in bgmBindings)
                Game.RegEvent(GameEventMap[bgmBinding.gameEvent],
                              _ =>
                              {
                                  Play(Types.BGM, BgmSo.GetClip(bgmBinding.SFX));
                              });
            Game.RegEvent(GameEvent.Game_Playing, _ => PlayingRoutine = Game.StartCoService(PlayerStep));
            //Game.RegEvent(GameEvent.Game_End,_=> Game.StopCoService(PlayingRoutine));
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
        [Serializable] class BGMBinding
        {
            [ValueDropdown("@((AudioComponent)$property.Tree.WeakTargets[0]).GetGameEvents()")] public string gameEvent;
            [ValueDropdown("@((AudioComponent)$property.Tree.WeakTargets[0]).GetBGMNames()")] public string SFX = PlayingNone;
        }
    }
}