using GameData;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    /// <summary>
    /// 游戏物件基类, 自动调用PlayerControlComponent的GameItemInteraction方法<br/>
    /// 之类只需要实现Invoke方法即可
    /// </summary>
    public abstract class GameItemBase : PlayerTrackingComponentBase,IGameItem
    {
        public abstract GameItemType Type { get; }
        /// <summary>
        /// 触发游戏物件
        /// </summary>
        /// <param name="player"></param>
        public abstract void Invoke(PlayableUnit player);

        protected override void OnPlayerTrackingEnter(PlayerControlComponent player) => player.GameItemInteraction(this);
        protected override void OnPlayerTrackingExit(PlayerControlComponent player) {  }
    }
    /// <summary>
    /// 碰撞检查组件基类
    /// </summary>
    public abstract class TrackingComponentBase : ColliderComponentBase
    {
        protected abstract void OnTrackingEnter(GameObject go);
        protected abstract void OnTrackingExit(GameObject go);

        protected override void OnCollider3DEnter(Collider col) => OnTrackingEnter(col.gameObject);
        protected override void OnCollider3DExit(Collider col) => OnTrackingExit(col.gameObject);
    }

    /// <summary>
    /// 检查玩家进入和离开的组件基类
    /// </summary>
    public abstract class PlayerTrackingComponentBase : TrackingComponentBase
    {
        protected abstract void OnPlayerTrackingEnter(PlayerControlComponent player);
        protected abstract void OnPlayerTrackingExit(PlayerControlComponent player);

        protected override void OnTrackingEnter(GameObject go)
        {
            var player = go.GetPlayerControlFromColliderHandler();
            if (player != null) OnPlayerTrackingEnter(player);
        }
        protected override void OnTrackingExit(GameObject go)
        {
            var player = go.GetPlayerControlFromColliderHandler();
            if (player != null) OnPlayerTrackingExit(player);
        }
    }
    /// <summary>
    /// 基于<seealso cref="Collider3DHandler"/>的碰撞处理组件基类
    /// </summary>
    public abstract class ColliderHandlerComponent : ColliderComponentBase
    {
        protected override void OnCollider3DEnter(Collider col) => CheckHandler(col, OnHandlerEnter);
        protected override void OnCollider3DExit(Collider col) => CheckHandler(col, OnHandlerExit);
        protected abstract void OnHandlerEnter(Collider3DHandler handler);
        protected abstract void OnHandlerExit(Collider3DHandler handler);
        void CheckHandler(Collider col, UnityAction<Collider3DHandler> onColliderAction)
        {
            var handler = col.GetComponent<Collider3DHandler>();
            if (!handler) return;
            onColliderAction(handler);
        }
    }
}