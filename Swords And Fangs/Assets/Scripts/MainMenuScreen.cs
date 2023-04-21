using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bestNightCounter;
    [SerializeField] Toggle mobileControllerToggle;

    public static MainMenuScreen Instance { get; private set; }

    PersistentData persistentData;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        persistentData = PersistentData.Instance;

        mobileControllerToggle.isOn = persistentData.MobileController;
    }

    public void RefreshBestNightCounter(bool bestNightIsLocked, int bestNight)
    {
        if (bestNightIsLocked)
            bestNightCounter.gameObject.SetActive(false);
        else
            bestNightCounter.text = "Best Night " + bestNight;
    }

    public void PlayBtn()
    {
        SceneLoader.Instance.LoadNextScene();
    }

    public void ToggleMobileController(bool value)
    {
        persistentData.MobileController = value;
    }
}