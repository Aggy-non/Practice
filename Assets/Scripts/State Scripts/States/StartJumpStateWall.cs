using UnityEngine;

public class StartJumpStateWall : IState
{
    private readonly StateCharacterControl player;
    public StartJumpStateWall(StateCharacterControl player)
    {
        this.player = player;

    }
    public void Enter()
    {
        player.Rb.linearVelocity = Vector3.zero; // will make movement close to 0
        CameraManager.Instance.MainCameraActive();
        //Debug.Log("Start Jump State");
    }
    public void Execute()
    {
        if (!player.WallDetected)
        {
            player.StateMachine.TransitionToState(player.StateMachine.airState);
        }
    }
    public void Exit()
    {

    }
}

