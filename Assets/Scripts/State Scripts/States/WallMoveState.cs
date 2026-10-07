using UnityEngine;

public class WallMoveState : IState
{
    private readonly StateCharacterControl player;

    public WallMoveState(StateCharacterControl player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.SetSurfaceType(StateCharacterControl.Surface.Wall);
        Debug.Log("Wall Move State");
    }

    public void Execute()
    {
        if (player.Rb.linearVelocity == Vector3.zero)
        {
            player.StateMachine.TransitionToState(player.StateMachine.wallState);
        }
        if (player.IsGrounded)
        {
            player.StateMachine.TransitionToState(player.StateMachine.idleState);
        }
        if (!player.WallDetected)
        {
            player.StateMachine.TransitionToState(player.StateMachine.airState);
        }
    }

    public void Exit()
    {

    }
}
