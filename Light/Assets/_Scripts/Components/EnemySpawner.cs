using GameData;
using GMVC.Core;

namespace Components
{
    /// <summary>
    /// 怪物生成器
    /// </summary>
    public class EnemySpawner : RandomObjectSpawner<EnemyComponent>
    {
        protected override void Get(EnemyComponent e) => e.OnDeathEvent.AddListener(OnEnemyDeath);
        void OnEnemyDeath() => Game.SendEvent(GameEvent.Battle_Skeleton_Death);
        protected override void Recycle(EnemyComponent e) => e.OnDeathEvent.RemoveAllListeners();
    }
}