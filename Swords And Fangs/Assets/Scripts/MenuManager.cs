using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] AudioClip mainMenuTrack;

    AudioStation audioStation;

    MainMenuScreen menuScreen;

    public PersistentData PersistentData { get; private set; }

    void Awake()
    {
        SceneManager.LoadSceneAsync((int)SceneIndexes.PERSISTENT_SCENE, LoadSceneMode.Additive);
    }

    void Start()
    {
        PersistentData = PersistentData.Instance;

        menuScreen = MainMenuScreen.Instance;
        menuScreen.RefreshBestNightCounter(
            PersistentData.BestNightIsLocked = PersistentData.LoadBestNightIsLocked() == 1 ? true : false,
            PersistentData.BestNight = PersistentData.LoadBestNight());

        audioStation = AudioStation.Instance;
        audioStation.StartNewMusicPlayer(mainMenuTrack, true);
    }
}