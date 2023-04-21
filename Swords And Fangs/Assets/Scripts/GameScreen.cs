using UnityEngine;

public class GameScreen : MonoBehaviour
{
    public GameGUI gameGUI;
    public FountainGUI fountainGUI;
    public GameOverGUI gameOverGUI;
    public PauseMenuGUI pauseMenuGUI;
    GameObject gameGUIObj, fountainGUIObj, gameOverGUIObj;
    GameObject currentGUIObj;

    private bool gamePaused = false;

    public static GameScreen Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        gameGUIObj = gameGUI.gameObject;
        fountainGUIObj = fountainGUI.gameObject;
        gameOverGUIObj = gameOverGUI.gameObject;
    }

    private void Update()
    {
        Escape();
    }

    public void OpenGameGUI()
    {
        OpenGUI(gameGUIObj);
    }

    public void OpenFountainGUI()
    {
        OpenGUI(fountainGUIObj);
        fountainGUI.UpdateGUI();
    }

    public void OpenGameOverGUI()
    {
        OpenGUI(gameOverGUIObj);
    }

    public void OpenPauseMenu()
    {
        pauseMenuGUI.PauseMenuOn();
    }

    void OpenGUI(GameObject GUIToOpen)
    {
        if (currentGUIObj)
            currentGUIObj.SetActive(false);
        currentGUIObj = GUIToOpen;
        GUIToOpen.SetActive(true);
    }

    public void Escape()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gamePaused)  //game pause using keyboard
        {
            OpenPauseMenu();
            gamePaused = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gamePaused)
        {
            gamePaused = false;
            pauseMenuGUI.Resume();
        }
    }
}