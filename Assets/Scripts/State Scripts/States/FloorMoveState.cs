using UnityEngine;

public class FloorMoveState : IState
{
    private readonly StateCharacterControl player;
    private InputManager input;
    public FloorMoveState(StateCharacterControl player,InputManager input)
    {
        this.player = player;
        this.input = input;//if you are not using this remove it later
    }
    public void Enter()
    {
        player.SetSurfaceType(StateCharacterControl.Surface.Ground);
        Debug.Log("floor Move State");
    }
    public void Execute()
    {
        if (player.Rb.linearVelocity== Vector3.zero)
        {
            player.StateMachine.TransitionToState(player.StateMachine.idleState);
        }

        if (!player.IsGrounded)
        {
            player.StateMachine.TransitionToState(player.StateMachine.airState);
        }
    }
    public void Exit()
    {

    }
}
