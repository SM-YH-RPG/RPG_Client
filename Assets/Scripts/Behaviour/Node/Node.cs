using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    public enum EState
    {
        Running,
        Success,
        Failure,
    }

    protected Node _parent = null;
    protected List<Node> childList = new List<Node>();

    public virtual void OnEnter() { }
    public virtual void OnExit() { }

    public abstract EState Execute();

    public virtual void AddChild(Node _child)
    {
        _child._parent = this;
        childList.Add(_child);
    }

    public virtual void RemoveChild(Node _child)
    {
        _child._parent = null;
        childList.Remove(_child);
    }

    public virtual void AddChildren(List<Node> _childs)
    {
        childList.AddRange(_childs);
    }

    public virtual void RemoveChildren(List<Node> _childs)
    {
        foreach (var child in _childs)
        {
            childList.Remove(child);
        }
    }

    public Node GetParent()
    {
        return _parent;
    }

    public Node GetChild(int _index)
    {
        return childList[_index];
    }

    public List<Node> GetChildren()
    {
        return childList;
    }

    protected bool HasChild()
    {
        return childList.Count > 0;
    }
}