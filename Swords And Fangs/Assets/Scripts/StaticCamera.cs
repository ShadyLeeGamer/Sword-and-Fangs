using MoreMountains.Feedbacks;
using UnityEngine;

public class StaticCamera : MonoBehaviour
{
    //Transform target;
    //[SerializeField] Vector3 offset;
    MMFeedbacks Shaker;

    public static StaticCamera Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        Shaker = GetComponent<MMFeedbacks>();
    }

    //void Start()
    //{
    //    target = DraculaController.Instance.transform; 
    //}

    //void LateUpdate()
    //{
    //    if (target)
    //        transform.position = target.position + offset;
    //}
    public void Shake()
    {
        Shaker.PlayFeedbacks();
    }
}