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
        player.SetMoveSpeedMultiplier(CharacterConstants.StartJumpMultiplier);
        player.SetDrag(CharacterConstants.StartJumpDrag);
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