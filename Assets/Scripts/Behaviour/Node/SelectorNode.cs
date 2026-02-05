using System.Collections.Generic;

public sealed class SelectorNode : Node
{
    public SelectorNode(Node parent, List<Node> _childs)
    {
        parent.AddChild(this);
        if (_childs == null)
        {
            return;
        }

        childList.AddRange(_childs);
    }

    public SelectorNode(List<Node> _childs)
    {
        if(_childs == null)
        {
            return;
        }

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
                    return EState.Success;
            }
        }

        return  EState.Failure;
    }
}