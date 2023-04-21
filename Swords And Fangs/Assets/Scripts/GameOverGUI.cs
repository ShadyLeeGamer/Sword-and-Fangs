using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverGUI : MonoBehaviour
{
    [SerializeField] string gameSceneName, mainMenuSceneName;

    [SerializeField] TextMeshProUGUI nightCounter, bestNightCounter;

    SceneLoader sceneLoader;

    void Start()
    {
        sceneLoader = SceneLoader.Instance;
    }

    public void RefreshNightCounters(int currentNight, int bestNight)
    {
        nightCounter.text = currentNight.ToString();
        bestNightCounter.text = bestNight.ToString();
    }

    public void TryAgainBTN()
    {
        SceneManager.LoadScene((int)SceneIndexes.GAME);
    }

    public void MainMenuBTN()
    {
        SceneManager.LoadScene((int)SceneIndexes.MAIN_MENU);
    }
}