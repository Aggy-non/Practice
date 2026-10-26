using UnityEngine;

public class WallAttachState : IState
{
    private readonly StateCharacterControl player;
    private Vector3 target;
    private Vector3 facing;
    private float timer;

    public WallAttachState(StateCharacterControl player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // nothing should push the player around while gliding to the wall
        player.SetGravity(0f);
        player.SetDrag(0f);
        player.SetMoveSpeedMultiplier(0f);
        player.SetSurfaceType(StateCharacterControl.Surface.Air); // not Wall yet, or SnapToWall would fight the glide

        target = player.GetWallAttachPosition();   // computed once, wallHit changes every frame
        facing = -player.WallNormal;
        timer = CharacterConstants.WallAttachTimeout;
        Debug.Log("Wall Attach State");
    }

    public void Execute()
    {
        player.SetFaceDir(facing); // stops movement input from turning you away mid-glide

        Vector3 toTarget = target - player.Rb.position;

        if (toTarget.magnitude <= CharacterConstants.WallAttachArriveDistance)
        {
            player.Rb.linearVelocity = Vector3.zero;
            player.StateMachine.TransitionToState(player.StateMachine.wallState);
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            player.StateMachine.TransitionToState(player.StateMachine.airState); // never got there
            return;
        }

        // capped at the attach speed, and never more than reaches the target in one physics step
        player.Rb.linearVelocity = Vector3.ClampMagnitude(toTarget / Time.fixedDeltaTime, CharacterConstants.WallAttachSpeed);
    }

    public void Exit() { }
}