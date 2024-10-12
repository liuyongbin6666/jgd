using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StoryOpen", menuName = "配置/故事开始")]

public class StoryOpenDataSO : ScriptableObject
{
    [SerializeField] private StoryOpenData[] storyOpenDatas;

    public StoryOpenData GetStoryOpenData(int id)
    {
        foreach (var storyOpenData in storyOpenDatas)
        {
            if (id==storyOpenData.id)
            {
                return storyOpenData;
            }
        }

        throw new InvalidOperationException();
    }
}


[Serializable]public class StoryOpenData
{
    public int id;
    public string open;
    public string[] body;
    public int[] storyFinishId;
}