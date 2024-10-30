using System;
using System.Collections;
using GameData;
using GMVC.Core;
using GMVC.Utls;
using MyBox;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utls;
using Random = UnityEngine.Random;

namespace Components
{
    /// <summary>
    /// 环境控件，主要控制闪雷
    /// </summary>
    public class EnvironmentComponent : CountdownComponent
    {
        [SerializeField] GameLaunch _gameLaunch;
        protected override int PulseTimes { get; } = 1;
        protected override float Duration => _duration;
        [SerializeField] Light directionalLight; // 3d全局方向光照组件
        [SerializeField, LabelText("闪电随机范围值")] MinMaxFloat _lightningRange;
        [SerializeField, LabelText("闪电时的光强度")] float lightningIntensity = 0.15f;
        [SerializeField, LabelText("闪电持续时间")] float lightningInterval = 0.15f;
        [SerializeField] RainingSet rainSet;
        public FollowComponent rain;
        float _duration = 3;

        public void Init()
        {
            OnCountdownComplete.AddListener(Lightning);
            RandomDuration();
            Game.RegEvent(GameEvent.Game_Playing,StartService);
            Game.RegEvent(GameEvent.Game_End,StopService);
        }
        void StopService(DataBag obj) => StopAllCoroutines();

        void StartService(DataBag obj)
        {
            StartCountdown(true);
            StartCoroutine(Raining());
        }

        IEnumerator Raining()
        {
            var maxRainingSecs = rainSet.maxRainingSecs;
            var minRainingSecs = rainSet.minRainingSecs;// 最低下雨时间
            var minClearSecs = rainSet.minClearSecs;// 最低停止下雨时间

            while (true)
            {
                // 开始下雨
                Rain(Game.World.Stage.Player.PlayerControl, true);
                yield return StartCoroutine(WaitWithRandomTime(minRainingSecs, maxRainingSecs));

                // 停止下雨
                Rain(Game.World.Stage.Player.PlayerControl, false);
                yield return StartCoroutine(WaitWithRandomTime(minClearSecs, maxRainingSecs));
            }
        }

        /// <summary>
        /// 等待随机时间，确保时间在最小值和最大值之间
        /// </summary>
        IEnumerator WaitWithRandomTime(float minTime, float maxTime)
        {
            var elapsedTime = 0f;
            var totalWaitTime = Random.Range(minTime, maxTime);

            while (elapsedTime < totalWaitTime)
            {
                yield return new WaitForSeconds(1);
                elapsedTime++;

                // 随着时间推移增加随机性，模拟降雨/晴天的自然结束
                var randomChance = Random.Range(0, elapsedTime + 1);
                var maxThreshold = Random.Range(0, (totalWaitTime / 2) + 1);

                if (randomChance > maxThreshold && elapsedTime >= minTime)
                    break;
            }
        }


        public void Rain(PlayerControlComponent player,bool enable)
        {
            rain.Display(enable);
            rain.SetFollow(player.transform);
            Game.SendEvent(GameEvent.Env_Rain_Update, enable);
        }

        void RandomDuration() => _duration = Random.Range(_lightningRange.Min, _lightningRange.Max);

        void Lightning()
        {
            if (Game.World.Status != GameWorld.GameStates.Playing) return;
            RandomDuration();
            var times = Random.Range(2, 4);
            StartCoroutine(LightningEffect(times, lightningInterval, StartCountdown));
        }

        IEnumerator LightningEffect(int times, float interval, UnityAction onLightningFinish)
        {
            // 保存原始光强度
            float originalIntensity = GetLightIntensity();
            var t = times;
            Game.SendEvent(GameEvent.Env_Lightning);
            while (t > 0)
            {
                yield return Lightning();
                t--;
            }
            onLightningFinish?.Invoke();
            yield break;

            IEnumerator Lightning()
            {
                // 设置全局光为闪电的光强度
                SetIntensity(lightningIntensity);
                // 闪电持续时间
                yield return new WaitForSeconds(interval);
                // 恢复原始光强度
                SetIntensity(originalIntensity);
                yield return new WaitForSeconds(interval / 3f);
            }
        }

        void SetIntensity(float intensity) => directionalLight.intensity = intensity;
        float GetLightIntensity() => directionalLight.intensity;

        [Serializable]
        class RainingSet
        {
            public float maxRainingSecs = 30f;
            public float minRainingSecs = 5f;  // 最低下雨时间
            public float minClearSecs = 5f;    // 最低停止下雨时间
        }
    }
}