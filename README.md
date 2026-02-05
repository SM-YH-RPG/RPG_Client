---
개별 작업
---
>**황성민 작업**
>
* FSM 구현
* BT 구현
* 플레이어 컨트롤러 구현
* 전투관련 시스템 구현
* 전투에 사용할 Scriptable Object 설계 및 구현
* 히트체크를 위한 HitBoxGenerator 구현
* 인벤토리 시스템 구현
* 아이템 장착 시스템 구현
* 리스폰 시스템 구현
* 풀링 시스템 구현
* 코딩 가이드 및 서포트 (데이터 구조, 각종 계산기, 파티 시스템 등)
>
---
폐기항목
---
* AWS에 게임서버 구축 [클라이언트 베이스로 전환하면서 폐기]
* 네트워크 통신 시스템 구현 (패킷핸들러, 디스패처 등) [클라이언트 베이스로 전환하면서 미사용]

>**양윤호 작업**
>
* 신규 플레이 데이터 생성 구현
* PlayerData JSON Export / Import 기능 구현
* PlayerData 저장 시점 설계 및 적용 (재화 변경, 액티브 파티 교체, 합성, 자동 저장(10분 주기), 즉시 저장(Pause/Quit))
* 월드 정보(NPC / Spawner / InteractableObject 배치) JSON Export 기능 구현
* 월드 JSON 데이터 기반 맵 진입 시 오브젝트 동적 생성 기능 구현
* 소모품 사용 & 쿨타임 기능 구현
* 캐릭터 StatCalculator / 캐릭터,몬스터 DamageCalculator 제작
* 캐릭터 레벨업 시스템 구현 (스탯 갱신, 레벨업 시 체력 회복 로직)
* 전투 이펙트 및 몬스터 Hit Effect AnimationEvent 기반 출력 기능 구현
* HUD DamageText 구현 및 Object Pooling 적용
* 캐릭터 사망 시 자동 스왑 기능 구현
* 파티 전멸 시 부활 팝업을 통한 전원 부활 플로우 구현
* 보스 몬스터 제작 및 Behavior Tree 적용
* 몬스터 리스폰 기능 적용
* 아이템(UI 출력 데이터), 무기, 장비, 소모품, 보스, 레벨, 스킬, 합성, 강화, 보상 ScriptableObject 기반 데이터 구조 설계 및 제작
* 모바일 Pinch Zoom 기능 구현
* SafeArea 제작 및 적용
* UI 제작 및 기능 구현
(캐릭터 정보, 인벤토리, 파티 교체, 무기/장비 교체, 강화, 합성, 보상, 상점, 결과, 부활, 토스트 메시지, Main HUD 등)
* 캐릭터 정보 팝업 내 캐릭터 Preview 기능 구현
* 캐릭터 스태미나 시스템 구현
* 테스트 내역 스프레드시트 작성
(https://docs.google.com/spreadsheets/d/1E7XLWfbbRwFrg4VLPxmtDnLbxqT1bu4oPXvQrlvpeQY/edit)
---
공동 작업
---
* PC / Mobile 빌드 테스트 & 디버그
