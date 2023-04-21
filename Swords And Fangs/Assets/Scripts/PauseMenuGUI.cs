using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuGUI : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject keepPlayingBTN;

    DraculaController dracula;
    AudioStation audioStation;

    public void PauseMenuOn()
    {
        if (!dracula)
        {
            dracula = DraculaController.Instance;
            audioStation = AudioStation.Instance;
        }
        dracula.CanControl = false;
        Time.timeScale = 0f;
        gameObject.SetActive(true);
        audioStation.ToggleAllPlayerPause(0);
    }

    public void Resume()
    {
        dracula.CanControl = true;
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        audioStation.ToggleAllPlayerPause(1);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        audioStation.ToggleAllPlayerPause(1);
        SceneManager.LoadScene((int)SceneIndexes.MAIN_MENU);
    }

    public void OptionsButton()
    {
        keepPlayingBTN.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
    }
    public void BackFromOptions()
    {
        keepPlayingBTN.gameObject.SetActive(true);
        optionsMenu.gameObject.SetActive(false);
    }
}