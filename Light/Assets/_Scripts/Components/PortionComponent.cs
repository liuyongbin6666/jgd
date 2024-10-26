using GameData;

namespace Components
{
    public class PortionComponent : GameItemBase
    {
        public int Hp;
        public override GameItemType Type { get; }= GameItemType.Portion_HP;
        public override void Invoke(PlayableUnit player)
        {
            player.Hp_Add(Hp);
            Destroy(gameObject);
        }
    }
}