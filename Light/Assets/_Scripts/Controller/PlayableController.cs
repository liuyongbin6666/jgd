using GameData;
using GMVC.Core;
using UnityEngine;
using Utls;

namespace Controller
{
    public class PlayableController : ControllerBase
    {
        GameWorld World => Game.World;
        PlayableUnit Player => World.Stage.Player;

        public void Move(Vector3 direction)
        {
            if (World.Status != GameWorld.GameStates.Playing) return;
            Player.Move(direction);
        }

        public void ChargeSpell(int spellId)
        {
            Player.ChargeSpell(spellId);
        }
    }
}