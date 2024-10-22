using System;
using UnityEngine;

namespace Config {
    [CreateAssetMenu(fileName = "StoryItem", menuName = "配置/故事道具")]

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
}