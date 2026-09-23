using System;
using UnityEngine;


[Serializable]
public class CharacterStateMachine
{
    public IState CurrentState { get; private set; }

    //states

    public event Action<IState> OnStateChanged;


    public CharacterStateMachine(StateCharacterControl player)
    {
        //this.walkState = new WalkState(player);
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
            CurrentState.Update();
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
