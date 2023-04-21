using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    Tombstone tombstone;
    SpawnManager spawnManager;
    Fountain fountain;
    GameCamera gameCam;
    DraculaController dracula;
    GameScreen gameScreen;
    AudioStation audioStation;
    PersistentData persistentData;

    public GameObject[] Particles;

    [SerializeField] AudioClip levelTrack;
    [SerializeField] AudioClip knightsAreComingSFX;

    public Animator draculaDashParticleAnimator, 
                    draculaShortAttack1ParticleAnimator,
                    draculaShortAttack2ParticleAnimator,
                    draculaBurstAttackParticleAnimator,
                    draculaChargeParticleAnimator;

    public bool NightIsPlaying { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tombstone = Tombstone.Instance;
        spawnManager = SpawnManager.Instance;
        fountain = Fountain.Instance;
        gameCam = GameCamera.Instance;
        gameScreen = GameScreen.Instance;
        audioStation = AudioStation.Instance;
        persistentData = PersistentData.Instance;

        if (!SceneLoader.Instance)
            StartGame();
    }

    public void StartGame()
    {
        tombstone.Initialise(dracula = tombstone.SummonDracula());
        fountain.Initialise(dracula);
        gameCam.Initialise(dracula);

        gameScreen.OpenGameGUI();
        persistentData.CurrentNight = 0;
        StartNextWave();
        audioStation.StartNewMusicPlayer(levelTrack, true);
        dracula.CanControl = true;
    }

    public void StartNextWave()
    {
        NightIsPlaying = true;
        foreach (GameObject particles in Particles)
            particles.SetActive(false);
        fountain.InteractBox.enabled = tombstone.InteractBox.enabled = false;
        fountain.UpdateRange(false);
        tombstone.UpdateRange(false);
        dracula.canLoseHealth = true;
        spawnManager.StartNextWave();

        gameScreen.gameGUI.RefreshNightCounters(persistentData.BestNightIsLocked,
                                                persistentData.CurrentNight + 1,
                                                persistentData.BestNight);
        gameScreen.gameGUI.knightsLeftCounter.gameObject.SetActive(true);
        gameScreen.gameGUI.RefreshKnightsLeftCounter(spawnManager.EnemiesRemaining);
        audioStation.StartNewSFXPlayer(knightsAreComingSFX, default, 1, 1, true);
    }

    public void EndWave()
    {
        NightIsPlaying = false;
        foreach (GameObject particles in Particles)
            particles.SetActive(true);
        dracula.ResetShortAttackCombo();
        dracula.canLoseHealth = false;

        fountain.InteractBox.enabled = tombstone.InteractBox.enabled = true;

        persistentData.CurrentNight += 1;
        persistentData.RefreshBestAttempt();

        gameScreen.gameGUI.knightsLeftCounter.gameObject.SetActive(false);
        gameScreen.gameGUI.RefreshNightCounters(persistentData.BestNightIsLocked,
                                                persistentData.CurrentNight,
                                                persistentData.BestNight);
    }

    public void GameOver()
    {
        gameScreen.OpenGameOverGUI();

        persistentData.BestNightIsLocked = false;
        persistentData.SaveBestNightIsLocked(0);
        persistentData.RefreshBestAttempt();

        gameScreen.gameOverGUI.RefreshNightCounters(persistentData.CurrentNight,
                                                    persistentData.BestNight);
    }
}