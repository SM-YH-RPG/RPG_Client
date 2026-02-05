using System;

public sealed class ActionNode : Node
{
    private Func<EState> OnUpdate = null;

    public ActionNode(Func<EState> _onUpdate)
    {
        OnUpdate = _onUpdate;
    }

    public override EState Execute() => OnUpdate?.Invoke() ?? EState.Failure;
}