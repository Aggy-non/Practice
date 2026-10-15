using UnityEngine;

public class ChargeState : IState
{
    private readonly StateCharacterControl player;
    public ChargeState(StateCharacterControl player)
    {
        this.player = player;

    }
    public void Enter()
    {
        player.SetDrag(CharacterConstants.ChargeJumpAirDrag);
        player.SetSurfaceType(StateCharacterControl.Surface.Air);
        CameraManager.Instance.AimCameraActive();
        Debug.Log("Charge state");
    }
    public void Execute()
    {
        if (!player.IsGrounded)
        {
            player.StateMachine.TransitionToState(player.StateMachine.airState);
        }

    }
    public void Exit()
    {

    }
}

