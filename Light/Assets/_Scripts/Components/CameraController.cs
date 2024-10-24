using GameData;
using GMVC.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components
{
    public class CameraController : MonoBehaviour
    {
        public float Speed=0.75f;
        public PlayerControlComponent playerControl;
        Transform player => playerControl.transform;
        public Vector3 Offset;
        [SerializeField, LabelText("采用动态镜头跟踪")] public bool dynamics;
        private void Awake()
        {
            Game.RegEvent(GameEvent.Game_Stage_Update, _ => playerControl = Game.World.Stage.Player.PlayerControl);
            Offset = transform.position-player.position;
        }
        private void LateUpdate()
        {
            if(player!=null)
            {
                if (dynamics)
                    transform.position = Vector3.Lerp(transform.position, (player.position + Offset), Speed * Time.deltaTime);
                else
                    transform.position = player.position;
            }
        }
    }
}
