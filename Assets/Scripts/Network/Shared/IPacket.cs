public enum ENetworkStatusCode
{
    None = 0,               

    Success = 100,          


    BadRequest = 400,                   // 요청 구문이 잘못됨 (예: 패킷 구조 오류)
    Unauthorized = 401,                 // 인증되지 않은 사용자 (로그인 필요)
    Forbidden = 403,                    // 권한 없음 (로그인했지만 해당 행동 금지)
    NotFound = 404,                     // 찾을 수 없음 (예: 아이템 템플릿 ID 없음, Empty 대신 사용)
    TooManyRequests = 429,              // 요청 빈도 초과 (도배 방지)


    InternalServerError = 500,          // 일반 서버 내부 오류 (ServerError 대신 사용)
    DatabaseError = 501,       

    AuthFailureInvalidUser = 601,       // 로그인 실패: 유효하지 않은 사용자
    AuthFailureWrongPassword = 602,     // 로그인 실패: 비밀번호 오류
    AuthFailureAlreadyLoggedIn = 603,   // 로그인 실패: 이미 로그인된 상태

    InventoryFull = 610,                // 인벤토리 공간 부족
    InsufficientFunds = 611,            // 재화 부족 (골드, 캐시 등)
    InvalidOperation = 612,             // 현재 상태에서 불가능한 작업 (예: 전투 중 채집 시도)
    InvalidTarget = 613,                // 상호작용 대상이 유효하지 않거나 찾을 수 없음
    OperationFailure = 614,             // 일반적인 작업 실패 (Specific code for general failure)
    NotEquippable = 615,                // 장착할 수 없는 아이템
    PartyNotFound = 616,                // 파티를 찾을 수 없음
}

public interface IPacket
{
    string PacketType { get; set; }
    public string GetResponseMessage();
}
