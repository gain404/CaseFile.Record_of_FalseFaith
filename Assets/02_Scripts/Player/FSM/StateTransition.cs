using System;

public class StateTransition
{
    public IState From;
    public IState To;

    public Func<bool> Condition;

    public StateTransition(IState from, IState to, Func<bool> condition)
    {
        From = from;
        To = to;
        Condition = condition;
    }
}
