using UnityEngine;

public class AlwaysBehind : MonoBehaviour
{
    public int sortOrder;

    // Use this for initialization
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.sortingLayerName = "Default";
        mr.sortingOrder = sortOrder;
    }
}