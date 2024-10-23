using GameData;
using UnityEngine;
using Utls;

namespace Components
{
    public class IdleState : IGameUnitState
    {
        PlayerControlComponent player;

        public IdleState(PlayerControlComponent player)
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
                player.SwitchState(new MoveState(player));
            }
            else if (player.Target && player.IsCdDone)
            {
                player.SwitchState(new AttackState(player));
            }
        }

        public void ExitState()
        {
            // 离开Idle状态时的处理
        }
    }
    public class MoveState : IGameUnitState
    {
        PlayerControlComponent player;

        public MoveState(PlayerControlComponent player)
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
                player.SwitchState(new IdleState(player));
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
    public class AttackState : IGameUnitState
    {
        PlayerControlComponent player;
        public AttackState(PlayerControlComponent player)
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
                player.SwitchState(new MoveState(player));
            }
            else if (player.anim.GetInteger(GameTag.AnimInt) == -1)
            {
                player.SwitchState(new IdleState(player));
            }
        }

        public void ExitState()
        {
            player.ResetAttackCD();
        }
    }
    public class ReactState : IGameUnitState
    {
        PlayerControlComponent player;
        float reactDuration = 0.3f;
        float reactTimer;

        public ReactState(PlayerControlComponent player)
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
                player.SwitchState(new IdleState(player));
            }
        }

        public void ExitState()
        {
            // 离开React状态时的处理
        }
    }
}