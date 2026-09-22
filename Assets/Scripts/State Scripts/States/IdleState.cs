using UnityEngine;

public class IdleState : IState
{
    
    private readonly CharacterControl player;
    public IdleState(CharacterControl player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.SetGravity(CharacterConstants.LandGravityScale);
        player.SetDrag(CharacterConstants.LandDrag);
    }
    public void Update()
    {

    }
    public void Exit()
    {

    }
}
