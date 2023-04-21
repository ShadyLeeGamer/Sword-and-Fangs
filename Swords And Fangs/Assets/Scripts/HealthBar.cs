using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    [SerializeField] MMFeedbacks feedbacks;

    void Awake()
    {
        slider = GetComponent<Slider>();
        if (feedbacks)
            feedbacks.Initialization();
    }

    public void SetMaxHealth(int maxHealth)
    {
        SetHealth(slider.maxValue = maxHealth);
    }

    public void SetHealth(float health)
    {
        if (feedbacks)
            PlayFeedBacks(health);
        slider.value = health;
    }

    public void PlayFeedBacks(float health)
    {
        feedbacks.FeedbacksIntensity = (slider.value - health);
        if (feedbacks.FeedbacksIntensity < 0)
        {
            feedbacks.FeedbacksIntensity = feedbacks.FeedbacksIntensity * -1;
        }
        feedbacks.FeedbacksIntensity = feedbacks.FeedbacksIntensity / 10;
        feedbacks.PlayFeedbacks();
    }

    public void UpgradeCapacity(int percentInc)
    {
        SetMaxHealth((int)slider.maxValue + percentInc);
        transform.parent.localScale += Vector3.right * (percentInc / 100f) * .3f;
    }
}