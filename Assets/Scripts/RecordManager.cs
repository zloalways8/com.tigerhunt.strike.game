using UnityEngine;

public class RecordManager : MonoBehaviour
{
    public static RecordManager Instance { get; private set; }

    private int maxScore;
    private int levelWithMaxScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveRecord(int level, int score)
    {
        if (score > maxScore)
        {
            maxScore = score;
            levelWithMaxScore = level;
            PlayerPrefs.SetInt("MaxScore", maxScore);
            PlayerPrefs.SetInt("MaxScoreLevel", levelWithMaxScore);
            PlayerPrefs.Save();
        }
    }

    public int GetMaxScore() => maxScore;
    public int GetMaxScoreLevel() => levelWithMaxScore;

    private void Start()
    {
        // Загружаем данные при старте
        maxScore = PlayerPrefs.GetInt("MaxScore", 0);
        levelWithMaxScore = PlayerPrefs.GetInt("MaxScoreLevel", 0);
    }
}