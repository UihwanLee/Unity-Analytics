using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private Text playerLevelLabel;

    private void Update()
    {
        playerLevelLabel.text = "PlayerLevel: " + level.ToString();
    }

    public void OnClickLevelUp()
    {
        level++;

        // Analytics 이벤트 전송
        AnalyticsManager.Instance.OnPlayerLevelUp(level);
    }
}
