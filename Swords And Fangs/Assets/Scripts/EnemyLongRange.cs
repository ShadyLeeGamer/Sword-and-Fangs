using UnityEngine;

public class EnemyLongRange : MonoBehaviour
{
    [Header("Long Range Attack")]
    [SerializeField] float shootRangeMin;
    [SerializeField] float shootRangeMax;
    [SerializeField] float shootSpeed;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Vector3 projectileSpawnPoint;
    [SerializeField] float projectileSpeed;
    [SerializeField] int projectileDamage;
    float shootCountdown;
    [SerializeField] AudioClip[] AttackSFX;
    public bool IsShooting { get; private set; }
    Vector3 TargetPos => movement.TargetPos;
    float DistanceBetweenEnemyAndTarget => Vector3.Distance(transform.position, TargetPos);
    public bool inShootRange => DistanceBetweenEnemyAndTarget >= shootRangeMin &&
                                DistanceBetweenEnemyAndTarget <= shootRangeMax;
    BoxCollider projectileHitBox;
    public bool inShootLine => TargetPos.z >= transform.position.z - (projectileHitBox.size.z / 2f) &&
                               TargetPos.z <= transform.position.z + (projectileHitBox.size.z / 2f);

    EnemyMovement movement;

    Transform flipper;
    Animator animator;

    public void Initialise(EnemyMovement movement)
    {
        this.movement = movement;
        flipper = movement.flipper;
        animator = movement.Animator;

        projectileHitBox = projectilePrefab.GetComponent<BoxCollider>();
    }

    public void TryShoot()
    {
        if (inShootRange)
        {
            shootCountdown -= shootSpeed * Time.deltaTime;
            if (shootCountdown <= 0f)
            {
                Shoot();
                shootCountdown = 1f;
            }
        }
        else if (shootCountdown != 0f)
            shootCountdown = 0f;
    }

    void Shoot()
    {
        animator.SetBool("isShooting", IsShooting = true);
        AudioStation.Instance.StartNewRandomSFXPlayer(AttackSFX, transform.position, movement.PitchRange.x, movement.PitchRange.y);
    }

    void EndShoot()
    {
        animator.SetBool("isShooting", IsShooting = false);
    }

    void ShootProjectile()
    {
        int faceDir = flipper.rotation.y == 0f ? 1 : -1;
        movement.ObjectPooler
            .GetProjectile(projectilePrefab.id,
                           movement.LocalisePos(projectileSpawnPoint), flipper.rotation,
                           new ObjectData(projectileDamage, projectileSpeed, faceDir, false));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + projectileSpawnPoint, .2f);
    }
}