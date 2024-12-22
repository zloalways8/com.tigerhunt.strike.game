using TMPro;
using UnityEngine;

public class ActionStreamCoordinator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] stageDisplayText;
        
    private int activeStageID;
        
    private void Start()
    {
        activeStageID = PlayerPrefs.GetInt(OmniMetrics.ACTIVE_STAGEID, 0);
        for (int indexCounterTipe = 0; indexCounterTipe < stageDisplayText.Length; indexCounterTipe++)
        {
            stageDisplayText[indexCounterTipe].text = "Level " + activeStageID;
        }
    }

    public void CompleteVictory()
    {
        PlayerPrefs.SetInt(OmniMetrics.ACTIVE_STAGEID, activeStageID+1);
        PlayerPrefs.Save();
        ChronoPhaseOrchestrator stageManager = FindObjectOfType<ChronoPhaseOrchestrator>();
        stageManager.AchieveStageVictory(activeStageID);
    }
}