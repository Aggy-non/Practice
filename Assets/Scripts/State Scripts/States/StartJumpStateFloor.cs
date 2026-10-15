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
        player.SetDrag(CharacterConstants.AirDrag);
        player.Rb.linearVelocity = Vector3.zero; // will make movement close to 0
        CameraManager.Instance.MainCameraActive();
        Debug.Log("Start Jump State");
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

 