using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
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
        CameraManager.Instance.MainCameraActive();
        Debug.Log("IdleState");
    }
    public void Execute()
    {
        //if(Mathf.Abs(player.Rb.linearVelocity.x)>0.1f || Mathf.Abs(player.Rb.linearVelocity.z)> 0.1f)
        //{
        ////    Debug.Log(Mathf.Abs(player.Rb.linearVelocity.z));
        ////    Debug.Log(Mathf.Abs(player.Rb.linearVelocity.x));
        //    player.StateMachine.TransitionToState(player.StateMachine.floorMoveState);
        //}
        if (player.MoveDir.normalized.magnitude > 0.1f)
        {
            player.StateMachine.TransitionToState(player.StateMachine.floorMoveState);
        }

        //transition to air state
        if (!player.IsGrounded)
        {
            player.StateMachine.TransitionToState(player.StateMachine.airState);
        }

    }
    public void Exit()
    {

    }
}
