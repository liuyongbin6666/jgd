using GameData;
using GMVC.Core;
using GMVC.Utls;

namespace Components
{
    /// <summary>
    /// 萤火虫生成器
    /// </summary>
    public class FireflySpawner : RandomObjectSpawner<FireflyComponent>
    {
        int tempMax;
        public float rainingRatio = 0.5f;
        string EventKey { get; set; }
        protected override void OnStartService()
        {
            tempMax = maxObjects;
            EventKey = Game.RegEvent(GameEvent.Env_Rain_Update,OnRaining);
        }
        protected override void OnStopService()
        {
            Game.RemoveEvent(GameEvent.Env_Rain_Update, EventKey);
        }

        void OnRaining(DataBag bag)
        {
            var isRaining = bag.Get<bool>(0);
            maxObjects = isRaining ? (int)(maxObjects * rainingRatio) : tempMax;
        }

        protected override void Get(FireflyComponent obj) => obj.RandomSet();
        protected override void Recycle(FireflyComponent obj) { }
    }
}