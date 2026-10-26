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
        CameraManager.Instance.WallCameraActive();
        Debug.Log("Wall State");
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
        if (player.MoveDir.sqrMagnitude > 0.01f)
        {
            player.StateMachine.TransitionToState(player.StateMachine.wallMoveState);
        }
    }

    public void Exit() { }
}