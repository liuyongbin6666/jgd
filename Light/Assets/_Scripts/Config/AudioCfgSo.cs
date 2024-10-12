using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioCfgSo", menuName = "配置/声效so")]
public class AudioCfgSo : ScriptableObject
{
    [SerializeField]List<AudioClip> clips = new();
    public string[] GetNames() => clips.Select(clip => clip.name).ToArray();
    public AudioClip GetClip(string clipName)=> clips.FirstOrDefault(clip => clip.name == clipName);
}
