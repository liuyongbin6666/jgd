using System.Linq;
using GameData;
using UnityEngine;
using Utls;

namespace Components
{
    public class PlayerIdleState : IGameUnitState
    {
        PlayerControlComponent player;

        public PlayerIdleState(PlayerControlComponent player)
        {
            this.player = player;
        }

        public void EnterState()
        {
            player.UpdateAnim(IGameUnitState.Anims.Idle);
        }

        public void UpdateState()
        {
            if (player.IsMoving)
            {
                player.SwitchState(new PlayerMoveState(player));
            }
            else if (player.Targets.Any() && player.IsCdDone)
            {
                player.SwitchState(new PlayerAttackState(player));
            }
        }

        public void ExitState()
        {
            // 离开Idle状态时的处理
        }
    }
    public class PlayerMoveState : IGameUnitState
    {
        PlayerControlComponent player;

        public PlayerMoveState(PlayerControlComponent player)
        {
            this.player = player;
        }

        public void EnterState()
        {
            player.UpdateAnim(IGameUnitState.Anims.Move);
        }

        public void UpdateState()
        {
            if (!player.IsMoving)
            {
                player.SwitchState(new PlayerIdleState(player));
            }
            else
            {
                player.HandleMovement();
            }
        }

        public void ExitState()
        {
            // 离开Move状态时的处理
        }
    }
    public class PlayerAttackState : IGameUnitState
    {
        PlayerControlComponent player;
        public PlayerAttackState(PlayerControlComponent player)
        {
            this.player = player;
        }

        public void EnterState()
        {
            player.UpdateAnim(IGameUnitState.Anims.Attack);
        }

        public void UpdateState()
        {
            if (player.IsMoving)
            {
                player.SwitchState(new PlayerMoveState(player));
            }
            else if (player.anim.GetInteger(GameTag.AnimInt) == -1)
            {
                player.SwitchState(new PlayerIdleState(player));
            }
        }

        public void ExitState()
        {
            player.ResetAttackCD();
        }
    }
    public class PlayerReactState : IGameUnitState
    {
        PlayerControlComponent player;
        float reactDuration = 0.3f;
        float reactTimer;

        public PlayerReactState(PlayerControlComponent player)
        {
            this.player = player;
        }

        public void EnterState()
        {
            player.SetInjured(true);
            player.stopMoving = true;
            reactTimer = 0f;
        }

        public void UpdateState()
        {
            reactTimer += Time.deltaTime;
            if (reactTimer >= reactDuration)
            {
                player.SetInjured(false);
                player.stopMoving = false;
                player.SwitchState(new PlayerIdleState(player));
            }
        }

        public void ExitState()
        {
            // 离开React状态时的处理
        }
    }
}