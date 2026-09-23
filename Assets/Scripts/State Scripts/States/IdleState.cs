using UnityEngine;

public class IdleState : IState
{
    
    private readonly StateCharacterControl player;
    public IdleState(StateCharacterControl player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.SetGravity(CharacterConstants.LandGravityScale);
        player.SetDrag(CharacterConstants.LandDrag);
        player.SetSurfaceType(StateCharacterControl.Surface.Ground);
        player.SetMoveSpeed(CharacterConstants.MoveSpeed);
        player.SetMoveSpeedMultiplier(CharacterConstants.MoveSpeedMultiplier);
    }
    public void Update()
    {

    }
    public void Exit()
    {

    }
}
