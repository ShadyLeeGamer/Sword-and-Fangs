using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] GameObject loadingScreen;
    [SerializeField] Slider progressBar;
    [SerializeField] Button summonBtn;
    [SerializeField] Sprite summonBtnDisabledSprite, summonBtnEnabledSprite;

    GameManager gameManager;

    void Awake()
    {
        Instance = this;
    }

    float totalSceneProgress;
    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void LoadNextScene()
    {
        summonBtn.interactable = false;
        summonBtn.image.sprite = summonBtnDisabledSprite;
        loadingScreen.SetActive(true);

        StartCoroutine(StartLoading());
    }

    IEnumerator StartLoading()
    {
        if (!SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MAIN_MENU).isLoaded)
        {
            scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.GAME));
            scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN_MENU, LoadSceneMode.Single));
        }
        else
        {
            scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MAIN_MENU));
            scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.GAME, LoadSceneMode.Additive));
        }

        yield return StartCoroutine(GetSceneLoadProgress());
    }

    public IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;
                foreach (AsyncOperation operation in scenesLoading)
                    totalSceneProgress += operation.progress;
                totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100f; // GET PERCENTAGE
                progressBar.value = Mathf.RoundToInt(totalSceneProgress);

                yield return null;
            }

        if (!gameManager)
            gameManager = GameManager.Instance;
        summonBtn.interactable = true;
        summonBtn.image.sprite = summonBtnEnabledSprite;
    }

    public void StartGame()
    {
        summonBtn.interactable = false;
        loadingScreen.SetActive(false);
        SceneManager.UnloadSceneAsync((int)SceneIndexes.PERSISTENT_SCENE);

        if (SceneManager.GetSceneByBuildIndex((int)SceneIndexes.GAME).isLoaded)
            gameManager.StartGame();
    }
} 

public enum SceneIndexes
{
    MAIN_MENU = 0,
    GAME = 1,
    PERSISTENT_SCENE = 2
}