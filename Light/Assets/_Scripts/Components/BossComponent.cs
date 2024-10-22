using GameData;

namespace Components
{
    public class BossComponent : GameItemBase
    {
        public bool IsDeath;
        public override GameItemType Type => GameItemType.Boss;
        public override void Invoke(PlayableUnit player)
        {
        }
    }
}