using UnityEngine;
using UnityEngine.SceneManagement;

public class DimensionalShiftHandler : MonoBehaviour
{
    public void DisplayStartupScreen()
    {
        SceneManager.LoadScene(OmniMetrics.SceneLoadingMessage);
    }
    public void LoadGameplayScreen()
    {
        SceneManager.LoadScene(OmniMetrics.PrimaryGameScene);
    }
    
    public void LoadGameplayArcadeScreen()
    {
        SceneManager.LoadScene(OmniMetrics.PrimaryGameArcadeScene);
    }
        
}