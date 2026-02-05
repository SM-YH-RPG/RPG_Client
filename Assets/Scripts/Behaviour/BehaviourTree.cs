using UnityEngine;

public class BehaviourTree
{
    private Node rootNode;

    public BehaviourTree(Node _root)
    {
        rootNode = _root;
    }

    public void Run()
    {
        rootNode.Execute();
    }
}
