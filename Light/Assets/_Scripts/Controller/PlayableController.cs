using GMVC.Core;
using UnityEngine;
using Utls;

public class PlayableController : ControllerBase
{
    GameWorld World => Game.World;
    PlayableUnit Player => World.Stage.Player;

    public void Move(Vector3 direction)
    {
        if (direction != Vector3.zero && World.Status != GameWorld.GameStates.Playing)
            throw XArg.Exception(("游戏状态错误！"), new { World.Status });
        Player.Move(direction);
    }
}