using System.Collections.Generic;

public class StateMachine
{
    protected IState currentState;
    protected readonly List<StateTransition> transitions = new List<StateTransition>();

    public void AddTransition(StateTransition transition)
    {
        transitions.Add(transition);
    }
    
    public virtual void ChangeState(IState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState?.Enter();
    }

    public void HandleInput()
    {
        currentState?.HandleInput();
    }

    public virtual void Update()
    {
        currentState?.Update();
    }

    public void PhysicsUpdate()
    {
        currentState?.PhysicsUpdate();
    }
    
}
