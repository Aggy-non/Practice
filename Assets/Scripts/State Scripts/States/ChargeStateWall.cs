using UnityEngine;

public class ChargeStateWall : IState
{
    private readonly StateCharacterControl player;

    public ChargeStateWall(StateCharacterControl player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // set everything explicitly so this state doesn't depend on the state before it
        player.SetSurfaceType(StateCharacterControl.Surface.Wall); // keeps SnapToWall running so you stay stuck
        player.SetGravity(CharacterConstants.WallGravityScale);
        player.SetDrag(CharacterConstants.WallDrag);
        player.SetMoveSpeedMultiplier(CharacterConstants.StartJumpMultiplier); // no strafing while charging
        CameraManager.Instance.AimCameraActive();
        Debug.Log("Charge state wall");
    }

    public void Execute()
    {
        // wall gone: stop holding the wall surface
        if (!player.WallDetected)
        {
            player.StateMachine.TransitionToState(player.StateMachine.airState);
        }
    }

    public void Exit() { }
}