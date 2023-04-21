using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public int currentNight;
    public int CurrentNight { get { return currentNight; }
                              set { currentNight = value; } }
    public int bestNight;
    public int BestNight { get { return bestNight; }
                           set { bestNight = value; } }
    public bool bestNightIsLocked = true;
    public bool BestNightIsLocked { get { return bestNightIsLocked; }
                                    set { bestNightIsLocked = value; } }

    public bool MobileController { get; set; }

    public static PersistentData Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
            PlayerPrefs.DeleteAll();
#endif
    }

    public void RefreshBestAttempt()
    {
        if (!BestNightIsLocked)
            if (currentNight > bestNight)
                SaveBestNight(bestNight = currentNight);
    }

    public int LoadBestNight()
    {
        return PlayerPrefs.GetInt("BestNight", 0);
    }

    public void SaveBestNight(int value)
    {
        PlayerPrefs.SetInt("BestNight", value);
    }

    public int LoadBestNightIsLocked()
    {
        return PlayerPrefs.GetInt("BestNightIsLocked", 1);
    }

    public void SaveBestNightIsLocked(int value)
    {
        PlayerPrefs.SetInt("BestNightIsLocked", value);
    }
}