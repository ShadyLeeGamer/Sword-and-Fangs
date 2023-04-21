using UnityEngine;

public class ViewBox : MonoBehaviour
{
    EnemyMovement enemy;

    void Awake()
    {
        enemy = transform.root.GetComponent<EnemyMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DraculaController>())
            enemy.TargetInViewRange(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DraculaController>())
            enemy.TargetInViewRange(false);
    }
}