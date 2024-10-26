using System;
using GameData;
using GMVC.Core;
using GMVC.Utls;
using UnityEngine;
using UnityEngine.Events;
using Utls;

namespace Controller
{
    /// <summary>
    /// 成就系统
    /// </summary>
    public class AchievementSystem : GameStartInitializer
    {
        public static AchievementSystem Instance { get; private set; }
        public int SkeletonDeathCount { get; private set; }
        int _skeletonDeathCountReach = 1;
        bool _init;
        protected override void OnGameStart()
        {
            if (_init) Debug.Log("重复初始化！", this);
            if (Instance != null) Debug.LogError($"成就已存在:{Instance.name}", this);
            _init = true;
            Instance = this;
            Init();
        }

        public event UnityAction<int> OnSkeletonDeathCountReach;
        void Init()
        {
            Game.RegEvent(GameEvent.Game_StateChanged, _ =>
            {
                if(Game.World.Status == GameWorld.GameStates.Playing)
                    ResetSkeletonDeathCount();
            });
            Game.RegEvent(GameEvent.Battle_Skeleton_Death, OnSkeletonDeath);
        }
        void ResetSkeletonDeathCount() => SkeletonDeathCount = 0;
        void OnSkeletonDeath(DataBag b)
        {
            SkeletonDeathCount++;
            if(SkeletonDeathCount==_skeletonDeathCountReach)
                OnSkeletonDeathCountReach?.Invoke(SkeletonDeathCount);
        }
    }
}