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
        CameraManager.Instance.WallCameraActive();
        Debug.Log("Wall Move State");
    }

    public void Execute()
    {
        if (!player.WallDetected)
        {
            player.StateMachine.TransitionToState(player.StateMachine.airState);
            return;
        }
        if (player.IsGrounded)
        {
            player.StateMachine.TransitionToState(player.StateMachine.idleState);
            return;
        }
        if (player.MoveDir.sqrMagnitude < 0.01f)
        {
            player.StateMachine.TransitionToState(player.StateMachine.wallState);
        }
    }

    public void Exit() { }
}