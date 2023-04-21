using UnityEngine;

public class AttackBox : MonoBehaviour
{
    public int Damage { get; set; }

    Transform attackingCharacter;

    DraculaController dracula;

    void Awake()
    {
        attackingCharacter = transform.root;
        dracula = attackingCharacter.GetComponent<DraculaController>();
    }

    void OnTriggerEnter(Collider other)
    {
        Character otherCharacter = other.GetComponent<Character>();
        if (otherCharacter)
        {
            if (!dracula && other.GetComponent<EnemyMovement>())
                return;

            otherCharacter.TakeDamage(Damage);
        }
    }
}