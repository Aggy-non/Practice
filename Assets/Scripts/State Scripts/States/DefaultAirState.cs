using UnityEngine;

public class DefaultAirState : IState
{
    private readonly StateCharacterControl player;
    private InputManager input;
    private float enterTime;
    public DefaultAirState(StateCharacterControl player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.SetGravity(CharacterConstants.AirGravityScale);
        player.SetDrag(CharacterConstants.AirDrag);
        player.SetSurfaceType(StateCharacterControl.Surface.Air);
        player.SetMoveSpeedMultiplier(CharacterConstants.AirSpeedMultiplier);
        enterTime = Time.time;
        CameraManager.Instance.MainCameraActive();
        Debug.Log("default air state");
    }
    public void Execute()
    {
        // right after a launch IsGrounded can still read true for a frame or two, and a shot
        // aimed down or flat may never leave the ground, so only trust it after a short grace period
        if (Time.time - enterTime < CharacterConstants.AirLandingGrace) return;

        if (player.IsGrounded)
        {
            player.StateMachine.TransitionToState(player.StateMachine.idleState);
        }
    }
    public void Exit()
    {

    }
}
