using UnityEngine;

public class TreasureObject : InteractionObject
{
    [SerializeField]
    private int _treasureID = 0;
    public int TreasureID => _treasureID;

    //.. TODO :: 리워드 시스템 활용
    public override void Interact()
    {

    }
}