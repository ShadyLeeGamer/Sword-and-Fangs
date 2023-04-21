using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : Character, IPooledObject
{
    public int id;
    public NavMeshAgent PathFinder { get; private set; }

    public int bloodWorth;

    [Header("Movement")]
    [SerializeField] float moveSpeedWander;
    [SerializeField] float moveSpeedChase;
    [SerializeField] float pathRefreshSpeed;
    float pathRefreshCountdown = 1f;
    [SerializeField] bool wanderTest;
    [SerializeField] float centreRadius;
    [SerializeField] float wanderRadius;

    DraculaController dracula;
    public Vector3 TargetPos { get; private set; }

    bool isMoving;
    bool inViewRange;

    [SerializeField] ParticleEffect impactPrefab;
    [SerializeField] MMFeedbacks feedbacks;

    SpawnManager spawnManager;

    EnemyCloseRange closeRange;
    EnemyLongRange longRange;

    [SerializeField] AudioClip[] HurtSFX;
    [SerializeField] AudioClip[] TakeDamageSFX;
    public bool PlayDieSound;
    [SerializeField] AudioClip[] DieSFX;
    [SerializeField] AudioClip[] SpawnSFX;
    [SerializeField] bool spawnSFXIs2D;
    public Vector2 PitchRange;

    public bool IsTakingDamage;

    public override void Awake()
    {
        base.Awake();
        PathFinder = GetComponent<NavMeshAgent>();
    }

    public override void Start()
    {
        base.Start();

        spawnManager = SpawnManager.Instance;
        
        dracula = DraculaController.Instance;
        if (dracula)
            TargetPos = dracula.transform.position;

        closeRange = GetComponent<EnemyCloseRange>();
        if (closeRange)
            closeRange.Initialise(this);
        longRange = GetComponent<EnemyLongRange>();
        if (longRange)
            longRange.Initialise(this);

        AudioStation.StartNewSFXPlayer(
            SpawnSFX[0], transform.position,
            spawnSFXIs2D ? 1 : PitchRange.x, spawnSFXIs2D ? 1 : PitchRange.y, spawnSFXIs2D);
    }

    public void Initialise(ObjectData enemyData)
    {
        healthBar.SetMaxHealth(CurrentHealth = maxHealth);
        PathFinder.SetDestination(RandomWanderLocation());

        if (AudioStation)
            AudioStation.StartNewSFXPlayer(SpawnSFX[0], transform.position, PitchRange.x, PitchRange.y);
    }

    public override void Update()
    {
        base.Update();

        HandleStates();
    }

    void HandleStates()
    {
        if (inViewRange && dracula)
        {
            PathFinder.speed = moveSpeedChase;

            pathRefreshCountdown -= pathRefreshSpeed * Time.deltaTime;
            if (pathRefreshCountdown <= 0f)
            {
                UpdatePath();
                pathRefreshCountdown = 1f;
            }

            TryAppropriateAttack();
        }
        else if (PathFinder.remainingDistance <= PathFinder.stoppingDistance)
        {
            PathFinder.speed = moveSpeedWander;
            PathFinder.SetDestination(RandomWanderLocation());
        }

        if ((closeRange && !closeRange.IsAttacking) ||
            (longRange && !longRange.IsShooting))
            UpdateFaceDir(PathFinder.velocity.x);

        if (longRange)
            if (longRange.IsShooting)
                PathFinder.isStopped = true;
            else
                PathFinder.isStopped = false;

        if (PathFinder.velocity.magnitude < .15f && isMoving)
            Animator.SetBool("isMoving", isMoving = false);
        else if (PathFinder.velocity.magnitude >= .15f && !isMoving)
            Animator.SetBool("isMoving", isMoving = true);
    }

    Vector3 RandomWanderLocation()
    {
        float radius = wanderTest ? wanderRadius : centreRadius;
        var finalWanderPos = Vector3.zero;
        Vector3 randomWanderPos = transform.position + Random.insideUnitSphere * radius;
        if (NavMesh.SamplePosition(randomWanderPos, out NavMeshHit hit, radius, 1))
            finalWanderPos = hit.position;
        return finalWanderPos;
    }

    void UpdatePath()
    {
        if (longRange && longRange.inShootRange && !longRange.inShootLine)
            TargetPos = dracula.transform.position + LocaliseFlipPos(Vector3.left * 8f);
        else
            TargetPos = dracula.transform.position;

        Vector3 targetPos = new Vector3(TargetPos.x, 2f, TargetPos.z);
        PathFinder.SetDestination(targetPos);
    }

    void TryAppropriateAttack()
    {
        if (closeRange && closeRange.inAttackRange)
            closeRange.TryAttack();
        else if (longRange && longRange.inShootRange && longRange.inShootLine)
            longRange.TryShoot();
    }

    public void TargetInViewRange(bool inRange)
    {
        inViewRange = inRange;
    }

    public override void TakeDamage(int damage, bool forceDamage = false, bool playHurstFX = true)
    {
        IsTakingDamage = true;
        StartCoroutine(DamageCoolDown());
        Vector3 impactPos = 
            new Vector3(transform.position.x, transform.position.y + .3f, transform.position.z - 0.3f);
        ObjectPooler.GetEnemyImpact(impactPos, Quaternion.identity, default);
        feedbacks.PlayFeedbacks();
        AudioStation.StartNewRandomSFXPlayer(HurtSFX, transform.position,PitchRange.x,PitchRange.y);
        AudioStation.StartNewRandomSFXPlayer(TakeDamageSFX, transform.position, PitchRange.x, PitchRange.y);
        base.TakeDamage(damage); // CHECK FOR DEATH
        gameCam.Shake();
    }

    IEnumerator DamageCoolDown()
    {
        yield return new WaitForSeconds(.2f);
        IsTakingDamage = false;
    }

    public override void Die()
    {
        feedbacks.ResetFeedbacks();
        dracula.GainHealth(GetComponent<EnemyMovement>().bloodWorth);
        spawnManager.RecordEnemyDeath();
        if (PlayDieSound)
            AudioStation.StartNewRandomSFXPlayer(DieSFX, transform.position, PitchRange.x, PitchRange.y);

        ObjectPooler.RecycleEnemy(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (dracula && longRange && longRange.inShootRange && !longRange.inShootLine)
            Gizmos.DrawSphere(dracula.transform.position + LocaliseFlipPos(Vector3.left * 8f), .3f);
    }
}