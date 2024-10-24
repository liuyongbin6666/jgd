using GameData;

namespace Components
{
    public class BossComponent : GameItemBase
    {
        public bool IsDeath => Enemy == null;
        public EnemyComponent Enemy;
        public override GameItemType Type => GameItemType.Boss;
        public override void Invoke(PlayableUnit player)
        {
        }
    }
}