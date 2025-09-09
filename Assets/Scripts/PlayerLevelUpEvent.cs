using Unity.Services.Analytics;

public class PlayerLevelUpEvent : Event
{
    // 생성자: 이벤트 이름 등록
    public PlayerLevelUpEvent() : base("PlayerLevelUpEvent") { }

    // 이벤트 파라미터 정의
    public int playerLevel { set { SetParameter("playerLevel", value); } }
}
