using System;
using System.Collections.Generic;
using Godot;

public interface IState
{
    void Init(params object[] args);
    void HandleInput();
    void Process(float dt);
    void PhysicsProcess(float dt);
    void Exit();
}

public class EmptyState : IState
{
    public void Init(params object[] args) {}
    public void HandleInput() {}
    public void Process(float dt) {}

    public void PhysicsProcess(float dt) {}
    public void Exit() {}
}

public class StateMachine
{
    public void Add(String stateName, IState state) { states.Add(stateName, state); }
    public void HandleInput() { currentState.HandleInput(); }
    public void Process(float dt) { currentState.Process(dt); }
    public void PhysicsProcess(float dt) { currentState.PhysicsProcess(dt); }
    public T GetState<T>() where T : IState { return (T) currentState; }

    public IState ChangeState(String stateName, params object[] args)
    {
        currentState.Exit();

        currentState = states[stateName];
        currentState.Init(args);
        return currentState;
    }
    private Dictionary<String, IState> states = new Dictionary<string, IState>();
    private IState currentState = new EmptyState();
}
