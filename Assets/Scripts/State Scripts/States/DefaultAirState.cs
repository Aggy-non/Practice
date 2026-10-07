using UnityEngine;

public class DefaultAirState : IState
{
    private readonly StateCharacterControl player;
    private InputManager input;
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
        Debug.Log("default air state");
    }
    public void Execute()
    {
        if(player.IsGrounded)
        {
            player.StateMachine.TransitionToState(player.StateMachine.idleState);
        }
    }
    public void Exit()
    {

    }
}
