using UnityEngine;

public class SingletonParticleHolder : MonoBehaviour
{
    public static SingletonParticleHolder Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public Animator dash, shortAttack1, shortAttack2, burstAttackRelease;
}