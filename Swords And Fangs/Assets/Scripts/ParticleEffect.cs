using UnityEngine;

public class ParticleEffect : MonoBehaviour, IPooledObject
{
    ObjectPooler objectPooler;

    void Awake()
    {
        objectPooler = ObjectPooler.Instance;
        
    }

    public void Initialise(ObjectData enemyImpactData) { }

    public void End()
    {
        objectPooler.RecycleEnemyImpact(this);
    }
}