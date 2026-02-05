using UnityEngine;

public class UseCharacterExpItemRequest : RequestPacket
{
    public int CharacterId;
    public int UseItemInstanceId; // 한번에 여러 종류 아이템을 쓸 수 있나..? 있다면 ConsumeExpItemTemplate같은 클래스 만들어서 개수랑 묶어서 List처리..
    public int UseAmount;
}
