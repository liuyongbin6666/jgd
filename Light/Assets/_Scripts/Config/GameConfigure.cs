using Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Config
{
    public class GameConfigure : MonoBehaviour
    {
        public GameConfig GameConfig;
        [LabelText("游戏单位")]public Transform GameUnitTransform;
        [LabelText("萤火虫生成器")]public FireflySpawner FireflySpawner;
        [LabelText("怪物生成器")]public EnemySpawner EnemySpawner;
    }
}