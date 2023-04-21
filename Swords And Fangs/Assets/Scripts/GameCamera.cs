using MoreMountains.Feedbacks;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] MMFeedbacks Shaker, Teleporter;
    public bool IsStatic;
    public bool LockY;

    public static GameCamera Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Initialise(DraculaController dracula)
    {
        target = dracula.transform;
    }

    void LateUpdate()
    {
        if (!IsStatic)
        {
            if (target)
            {
                if (!LockY)
                {
                    transform.position = target.position + offset;
                }
                else
                {
                    transform.position = new Vector3(target.position.x + offset.x, offset.y, offset.z);
                }
            }
        }
    }

    public void Shake()
    {
        Shaker.PlayFeedbacks();
    }

    public void Teleport()
    {
        Teleporter.PlayFeedbacks();
    }
}