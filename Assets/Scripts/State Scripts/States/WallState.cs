using UnityEngine;

public class WallState : IState
{
    private readonly StateCharacterControl player;

    public WallState(StateCharacterControl player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.SetGravity(CharacterConstants.WallGravityScale);
        player.SetDrag(CharacterConstants.WallDrag);
        player.SetSurfaceType(StateCharacterControl.Surface.Wall);
        player.SetMoveSpeedMultiplier(CharacterConstants.DefaultMoveSpeedMultiplier);
        Debug.Log("Wall State");
    }

    public void Execute()
    {
        if (Mathf.Abs(player.Rb.linearVelocity.x) > 0.1f)
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
