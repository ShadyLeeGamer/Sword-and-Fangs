using UnityEngine;

public class EnemyCloseRange : MonoBehaviour
{
    [Header("Close Range Attack")]
    [SerializeField] float attackSpeed;
    [SerializeField] AttackBox attackBox;
    [SerializeField] int attackDamage;
    [SerializeField] AudioClip[] AttackSFX;
    float attackCountdown;

    public bool IsAttacking { get; private set; }
    public bool inAttackRange => Vector3.Distance(transform.position, movement.TargetPos)
                              <= movement.PathFinder.stoppingDistance;

    EnemyMovement movement;

    public void Initialise(EnemyMovement movement)
    {
        this.movement = movement;
        attackBox.Damage = attackDamage;
    }

    public void TryAttack()
    {
        if (inAttackRange)
        {
            attackCountdown -= attackSpeed * Time.deltaTime;
            if (attackCountdown <= 0f)
            {
                Attack();
                attackCountdown = 1f;
            }
        }
        else if (attackCountdown != 0f)
            attackCountdown = 0f;
    }

    void Attack()
    {
        movement.Animator.SetBool("isAttacking", IsAttacking = true);
        AudioStation.Instance.StartNewRandomSFXPlayer(AttackSFX, transform.position, movement.PitchRange.x, movement.PitchRange.y);
    }

    public void EndAttack()
    {
        movement.Animator.SetBool("isAttacking", IsAttacking = false);
    }
}