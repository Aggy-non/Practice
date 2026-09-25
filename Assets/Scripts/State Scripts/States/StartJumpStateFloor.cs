using UnityEngine;

public class StartJumpStateFloor : IState    
{
    private readonly StateCharacterControl player;
    public StartJumpStateFloor(StateCharacterControl player)
    {
        this.player = player;
       
    }
    public void Enter()
    {
        player.SetGravity(CharacterConstants.LandGravityScale);
        player.SetDrag(CharacterConstants.LandDrag);
        player.SetSurfaceType(StateCharacterControl.Surface.Ground);
        player.SetMoveSpeed(CharacterConstants.FloorMoveSpeed);
        player.SetMoveSpeedMultiplier(CharacterConstants.StartJumpMultiplier);
        //also freeze rotation here
        Debug.Log("floorState");
    }
    public void Execute()
    {
        //write transition code here
    }
    public void Exit()
    {

    }
}
