using System;
using UnityEngine;


[Serializable]
public class CharacterStateMachine
{
    public IState CurrentState { get; private set; }

    //states
    public IdleState idleState;
    public FloorMoveState floorMoveState;
    public StartJumpState startJumpState;
    public DefaultAirState airState;
    public ChargeState chargeState;
    public WallState wallState;
    public WallAttachState wallAttachState;
    public WallMoveState wallMoveState;

    public event Action<IState> OnStateChanged;


    public CharacterStateMachine(StateCharacterControl player,InputManager input)
    {
        this.idleState = new IdleState(player,input);
        this.floorMoveState = new FloorMoveState(player,input);
        this.startJumpState = new StartJumpState(player);
        this.airState = new DefaultAirState(player);
        this.chargeState = new ChargeState(player);
        this.wallState = new WallState(player);
        this.wallAttachState = new WallAttachState(player);
        this.wallMoveState = new WallMoveState(player);
        Debug.Log("state machine online");
    }
    public void Initialize(IState state)
    {
        CurrentState = state;
        state.Enter();

        OnStateChanged?.Invoke(state);
    }

    public void Exectue()
    {
        if (CurrentState != null)
        {
            CurrentState.Execute();
        }
        else
        {
            Debug.LogWarning("CurrentState is null. Cannot execute state logic.");
        }
    }

    public void TransitionToState(IState newState)
    {
        //Debug.Log("Transitioned to state: " + newState.GetType().Name);
        CurrentState.Exit();
        CurrentState = newState;
        newState.Enter();
        
        OnStateChanged?.Invoke(newState);
    }
}
