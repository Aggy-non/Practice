using UnityEngine;

public class IdleState : IState
{
    
    private readonly StateCharacterControl player;
    private InputManager input;
    public IdleState(StateCharacterControl player,InputManager input)
    {
        this.player = player;
        this.input = input;//if you are not using this remove it later
    }
    public void Enter()
    {
        player.SetGravity(CharacterConstants.LandGravityScale);
        player.SetDrag(CharacterConstants.LandDrag);
        player.SetSurfaceType(StateCharacterControl.Surface.Ground);
        player.SetMoveSpeedMultiplier(CharacterConstants.DefaultMoveSpeedMultiplier);
        Debug.Log("IdleState");
    }
    public void Execute()
    {
        if(player.Rb.linearVelocity != Vector3.zero)
        {
            player.StateMachine.TransitionToState(player.StateMachine.floorMoveState);
        }
    }
    public void Exit()
    {

    }
}
