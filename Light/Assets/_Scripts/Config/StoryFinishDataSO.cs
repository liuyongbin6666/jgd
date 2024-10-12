using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StoryFinish", menuName = "≈‰÷√/π  ¬Ω· ¯")]
public class StoryFinishDataSO : ScriptableObject
{
    [SerializeField] private StoryFinishData[] storyFinishDatas;

    public StoryFinishData GetStoryFinishData(int id)
    {
        foreach (var storyFinishData in storyFinishDatas)
        {
            if (id == storyFinishData.id)
            {
                return storyFinishData;
            }
        }

        throw new InvalidOperationException();
    }
}

[Serializable] public class StoryFinishData
{
    public int id;
    public string[] transition;
    public string[] finish;
    public int[] itemId;
}