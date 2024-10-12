using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryItem", menuName = "≈‰÷√/π  ¬µ¿æﬂ")]

public class StoryItemDataSO : ScriptableObject
{
    [SerializeField] private StoryItemData[] storyItemDatas;

    public StoryItemData GetStoryItemData(int id)
    {
        foreach (var storyItemData in storyItemDatas)
        {
            if (id==storyItemData.id)
            {
                return storyItemData;
            }
        }

        throw new InvalidOperationException();
    }
}

[Serializable] public class StoryItemData
{
    public int id;
    public string name;
    public string str;
}
