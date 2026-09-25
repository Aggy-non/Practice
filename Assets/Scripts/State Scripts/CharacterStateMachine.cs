using System;
using UnityEngine;


[Serializable]
public class CharacterStateMachine
{
    public IState CurrentState { get; private set; }

    //states
    public IdleState idleState;
    public FloorMoveState floorMoveState;
    public StartJumpStateFloor startJumpState;

    public event Action<IState> OnStateChanged;


    public CharacterStateMachine(StateCharacterControl player,InputManager input)
    {
        this.idleState = new IdleState(player,input);
        this.floorMoveState = new FloorMoveState(player,input);
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
        CurrentState.Exit();
        CurrentState = newState;
        newState.Enter();

        OnStateChanged?.Invoke(newState);
    }
}
