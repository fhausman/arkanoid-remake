using System;
using System.Collections.Generic;
using Godot;

public interface IState
{
    void Init();
    void HandleInput();
    void Process(float dt);
    void PhysicsProcess(float dt);
    void Exit();
}

public class EmptyState : IState
{
    public void Init() {}
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

    public IState ChangeState(String stateName)
    {
        currentState.Exit();

        currentState = states[stateName];
        currentState.Init();
        return currentState;
    }
    private Dictionary<String, IState> states = new Dictionary<string, IState>();
    private IState currentState = new EmptyState();
}
