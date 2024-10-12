using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public enum Types
    {
        BGM,
        SFX
    }
    [SerializeField] protected AudioSource bgm;
    [SerializeField] protected AudioSource sfx;
    AudioSource GetAudioSource(Types type)
    {
        return type switch
        {
            Types.BGM => bgm,
            Types.SFX => sfx,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public void Play(Types type, AudioClip clip, Action onCompleteCallback = null)
    {
        var audioSource = GetAudioSource(type);
        audioSource.clip = clip;
        StopCoroutine(OnComplete());
        StartCoroutine(OnComplete());

        IEnumerator OnComplete()
        {
            audioSource.Play();
            if (audioSource.loop && onCompleteCallback != null)
                throw new Exception("AudioSource is looped, onCompleteCallback will never be called");
            yield return new WaitWhile(() => audioSource.isPlaying);
            onCompleteCallback?.Invoke();
        }
    }


    public void Stop(Types type)=> GetAudioSource(type).Stop();
    public void SetVolume(Types type, float volume) => GetAudioSource(type).volume = volume;
    public void AddVolume(Types type, float volume) => GetAudioSource(type).volume += volume;
    public void MuteAll(bool mute)
    {
        bgm.mute = mute;
        sfx.mute = mute;
    }
    public void Mute(Types type, bool mute) => GetAudioSource(type).mute = mute;
    public void Pause(Types type, bool pause)
    {
        var audioSource = GetAudioSource(type);
        if (pause)
            audioSource.Pause();
        else
            audioSource.UnPause();
    }
    public void Set(Types type, Action<AudioSource> setFunc) => setFunc(GetAudioSource(type));
}
