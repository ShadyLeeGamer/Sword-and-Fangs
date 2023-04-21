using UnityEngine;
using TMPro;

public class GameGUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nightCounter1;
    [SerializeField] TextMeshProUGUI nightCounter2, bestNightCounter;
    public TextMeshProUGUI knightsLeftCounter;

    public void RefreshNightCounters(bool bestNightIsLocked, int currentNight, int bestNight)
    {
        if (bestNightIsLocked)
            RefreshNightCounter(nightCounter1, currentNight);
        else
        {
            RefreshNightCounter(nightCounter2, currentNight);
            bestNightCounter.text = "Best Night " + bestNight;
        }
        nightCounter1.transform.parent.gameObject.SetActive(bestNightIsLocked);
        nightCounter2.transform.parent.gameObject.SetActive(!bestNightIsLocked);
    }

    void RefreshNightCounter(TextMeshProUGUI nightCounter, int currentNight)
    {
        nightCounter.text = "Night " + currentNight;
    }

    public void RefreshKnightsLeftCounter(int knightsLeft)
    {
        knightsLeftCounter.text = knightsLeft + " Knights Left";
    }

    public void PauseBTN()
    {

    }
}