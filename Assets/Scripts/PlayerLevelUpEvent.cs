using Unity.Services.Analytics;

public class PlayerLevelUpEvent : Event
{
    // ������: �̺�Ʈ �̸� ���
    public PlayerLevelUpEvent() : base("PlayerLevelUpEvent") { }

    // �̺�Ʈ �Ķ���� ����
    public int playerLevel { set { SetParameter("playerLevel", value); } }
}
