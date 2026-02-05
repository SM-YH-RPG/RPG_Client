using UnityEngine;

public class RootNode : Node
{
    public override EState Execute()
    {
        if (!HasChild())
        {
            return EState.Failure;
        }

        Node childNode = GetChild(0);
        return childNode.Execute();
    }
}