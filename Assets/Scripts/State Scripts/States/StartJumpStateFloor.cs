using UnityEngine;

public class StartJumpState : IState    
{
    private readonly StateCharacterControl player;
    public StartJumpState(StateCharacterControl player)
    {
        this.player = player;
       
    }
    public void Enter()
    {
        player.SetGravity(CharacterConstants.AirGravityScale);
        player.SetMoveSpeedMultiplier(CharacterConstants.StartJumpMultiplier); // will make movement input 0
        player.SetDrag(CharacterConstants.StartJumpDrag); // will make movement close to 0
        Debug.Log("Start Jump State");
    }
    public void Execute()
    {
        //if (!player.IsGrounded)
        //{
        //    player.StateMachine.TransitionToState(player.StateMachine.airState);
        //}
    }
    public void Exit()
    {

    }
}

 