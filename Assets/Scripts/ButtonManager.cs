using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private int currentLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = 0;
    }

    public void NextLevel()
    {
        currentLevel++;
        Debug.Log("Click NextLevel");
        AnalyticsManager.Instance.NextLevel(currentLevel);
    }
}
