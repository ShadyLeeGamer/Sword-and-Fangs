using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{
    public int id;

    int damage;

    Rigidbody rb;

    bool isDraculaProjectile;

    ObjectPooler objectPooler;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    public void Initialise(ObjectData projectileData)
    {
        damage = projectileData.damage;
        isDraculaProjectile = projectileData.isDraculaProjectile;

        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.position + (Vector3.right * projectileData.speed * projectileData.faceDir));
    }

    void OnTriggerEnter(Collider other)
    {
        Character otherCharacter = other.GetComponent<Character>();
        if (otherCharacter)
        {
            if ((!isDraculaProjectile && other.GetComponent<EnemyMovement>()) ||
                isDraculaProjectile && !other.GetComponent<EnemyMovement>())
                return;

            otherCharacter.TakeDamage(damage);
            EndProjectile();
        }
    }

    public void EndProjectile()
    {
        rb.velocity = Vector3.zero;
        objectPooler.RecycleProjectile(this);
    }
}