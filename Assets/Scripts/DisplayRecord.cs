using TMPro;
using UnityEngine;

public class DisplayRecord : MonoBehaviour
{
    public TextMeshProUGUI recordText; // Ссылка на текстовый объект

    void Start()
    {
        // Получаем данные из RecordManager
        int maxScore = RecordManager.Instance.GetMaxScore();
        int level = RecordManager.Instance.GetMaxScoreLevel();

        // Обновляем текст
        recordText.text = $"Level {level-1} - {maxScore}";
    }
}