using Sirenix.OdinInspector;
using UnityEngine;

namespace Config
{
    public class GameConfigure : MonoBehaviour
    {
        public GameConfig GameConfig;
        [LabelText("游戏单位")]public Transform GameUnitTransform;
    }
}