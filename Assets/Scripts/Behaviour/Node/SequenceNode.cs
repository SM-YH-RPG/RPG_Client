using System.Collections.Generic;
using Unity.Transforms;

public class SequenceNode : Node
{
    public SequenceNode()
    {

    }

    public SequenceNode(List<Node> _childs)
    {
        if (_childs == null)
            return;

        childList.AddRange(_childs);
    }

    public override EState Execute()
    {
        if (childList == null || childList.Count == 0)
            return EState.Failure;

        foreach (var child in childList)
        {
            switch (child.Execute())
            {
                case EState.Running:
                    return EState.Running;
                case EState.Success:
                    continue;
                case EState.Failure:
                    return EState.Failure;
            }
        }

        return EState.Success;
    }
}