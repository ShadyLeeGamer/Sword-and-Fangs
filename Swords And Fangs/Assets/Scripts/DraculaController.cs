using System.Collections;
using UnityEngine;
//Adjusting the Game Feel a bit..
using MoreMountains;
using MoreMountains.Feedbacks;


public class DraculaController : Character
{
    Joystick mobileJoystick;

    public bool canLoseHealth;
    [SerializeField] float healthDecSpeed;
    float healthDecCountdown = 0f;
    [SerializeField] Color combatTextGainedColour;

    [Header("Movement")]
    [SerializeField] float moveSpeedX;
    [SerializeField] float moveSpeedZ;

    [Header("Short Attack")]
    [SerializeField] AttackBox shortAttackBox;
    [SerializeField] int shortAttackDamage;
    [SerializeField] float maxShortAttackComboDelay;
    [SerializeField] Vector3 shortAttackParticlePos;
    [SerializeField] Animator shortAttack1ParticleAnimator, shortAttack2ParticleAnimator;
    float lastShortAttackTime;
    public int ShortAttackComboNo { get; private set; }

    [Header("Dash")]
    [SerializeField] int dashCost;
    [SerializeField] float dashDuration;
    [SerializeField] float dashSpeed;
    [SerializeField] Vector3 dashParticlePos;
    [SerializeField] Animator dashParticleAnimator;
    [SerializeField] float dstBetweenRays = 0.24f;
    [SerializeField] LayerMask collisionMask = default;
    const float SKIN_WIDTH = 0.015f;
    int rayCountX = 4, rayCountZ;
    float raySpacingX, raySpacingZ;
    BoxCollider boxCollider;
    struct RaycastOrigins
    {
        public Vector3 backLeft, backRight;
        public Vector3 frontLeft, frontRight;
    }
    RaycastOrigins raycastOrigins;

    [Header("Burst Charge Attack")]
    [SerializeField] int burstChargeAttackCost;
    [SerializeField] AttackBox burstAttackBox;
    [SerializeField] int[] burstAttackDamages = new int[3];
    [SerializeField] float burstAttackChargeSpeed;
    [SerializeField] Vector3 burstAttackReleaseParticlePos;
    [SerializeField] Animator burstAttackReleaseParticleAnimator;
    float burstAttackChargeTime;
    float burstAttackDamageIncThreshold;

    [Header("Projectile Charge Attack")]
    [SerializeField] int projectileChargeAttackCost;
    [SerializeField] Vector3 projectileSpawnPoint;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] int projectileChargeSpeed;
    [SerializeField] int[] projectileAttackDamages = new int[3];
    [SerializeField] int projectileMoveSpeed;
    int projectileDamage;
    float projectileAttackChargeTime;
    float projectileAttackDamageIncThreshold;

    [SerializeField] Vector3 chargeParticlePos;
    [SerializeField] Animator chargeParticleAnimator;

    [Header("Blood Capacity Increase")]
    [SerializeField] int bloodCapacityIncPercent;
    [SerializeField] int bloodCapacityIncLevelCount;

    [Header("Blood Steal Increase")]
    [SerializeField] int bloodStealIncPercent;
    [SerializeField] int bloodStealIncLevelCount;

    [Header("Controls")]
    [SerializeField] KeyCode attackKey = KeyCode.O;
    int attackOnController = 0;
    bool mobileAttackButtonDown;
    public bool isAttacking;
    [SerializeField] KeyCode dashKey = KeyCode.Space;
    int DashOnController = 0;
    bool mobileDashButtonDown;
    const float ATTACK_KEY_HOLD_TRIGGER_THRESHOLD = .3f;
    float attackKeyHoldTime;
    bool attackKeyHold;
    bool controllerMobileAttackButtonUp;

    [Header("Sounds")]
    public AudioClip[] WalkSounds;
    public AudioClip[] PunchSounds;
    public AudioClip[] DashSounds;
    public AudioClip[] BurstSounds;
    public AudioClip[] HurtSounds;
    public AudioClip DieSound;
    public AudioClip[] ChargingSounds;

    [SerializeField]
    public MMFeedbacks DamageFeedback;

    [System.Serializable]
    public struct Unlockable
    {
        public string name;
        public bool isUnlocked;
        public int unlockCost;
    }

    [System.Serializable]
    public struct Buff
    {
        public Unlockable unlockable;
        public int incPercent;
        public int incLevelMax;
        public int incLevelCurrent;

        public bool IncLevelIsAtMax => incLevelCurrent == incLevelMax;
    }
    public Unlockable[] unlockableAbilities;
    public Buff[] unlockableBuffs;

    Rigidbody rb;

    Vector2 moveInput;
    Vector3 lastMoveDir = new Vector3(1f, 0f, 0f);

    bool isDashing;
    bool isChargingBurstAttack, isChargingProjectileAttack;
    bool isReleasingBurstAttack, isReleasingProjectileAttack;
    bool IsCharging => isChargingBurstAttack || isChargingProjectileAttack;
    bool IsReleasing => isReleasingBurstAttack || isReleasingProjectileAttack;
    public bool CanControl { get { return canControl; }
                             set { canControl = value; } }
    bool canControl = false;

    GameManager gameManager;
    MobileControls mobileControls;

    public static DraculaController Instance { get; private set; }

    public override void Awake()
    {
        base.Awake();

        Instance = this;

        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public override void Start()
    {
        base.Start();
        healthBar.SetMaxHealth(CurrentHealth = maxHealth);

        mobileControls = MobileControls.Instance;
        if (mobileControls)
            mobileControls.dracula = this;
        mobileJoystick = Joystick.Instance;

        CalculateRaySpacing(); /// NO RECALCULATION
        UpdateRaycastOrigins();
        UpdateFaceDir(moveInput.x);

        shortAttackBox.Damage = shortAttackDamage;
        burstAttackBox.Damage = burstAttackDamages[0];
        burstAttackDamageIncThreshold = 100f / burstAttackDamages.Length;
        projectileAttackDamageIncThreshold = 100f / projectileAttackDamages.Length;

        gameManager = GameManager.Instance;
        dashParticleAnimator = gameManager.draculaDashParticleAnimator;
        shortAttack1ParticleAnimator = gameManager.draculaShortAttack1ParticleAnimator;
        shortAttack2ParticleAnimator = gameManager.draculaShortAttack2ParticleAnimator;
        burstAttackReleaseParticleAnimator = gameManager.draculaBurstAttackParticleAnimator;
        chargeParticleAnimator = gameManager.draculaChargeParticleAnimator;
    }

    public override void Update()
    {
        base.Update();

        if (canLoseHealth && gameManager.NightIsPlaying)
        {
            healthDecCountdown -= healthDecSpeed * Time.deltaTime;
            if (healthDecCountdown <= 0f)
            {
                TakeDamage(1);
                healthDecCountdown = 1f;
            }
        }

        if (Time.time - lastShortAttackTime > maxShortAttackComboDelay && ShortAttackComboNo != 3)
            ResetShortAttackCombo();

        if (canControl)
        {
            ControllerInputs();
            RegisterInputs();
        }
    }

    void RegisterInputs()
    {
        if (isDashing || IsReleasing)
            return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        if (mobileJoystick)
        {
            moveInput.x += mobileJoystick.Horizontal;
            moveInput.y += mobileJoystick.Vertical;
        }

        CheckForAttackHoldKey();

        isAttacking = Input.GetKey(attackKey)/* || attackOnController == 2*/ || mobileAttackButtonDown;

        if (ShortAttackComboNo < 3)
        {
            if (ShortAttackComboNo > 0)
            {
                if (Input.GetKeyDown(attackKey)/* || attackOnController == 2*/ || mobileAttackButtonDown)
                {
                    ShortAttack();
                }
            }
            else if (unlockableAbilities[2].isUnlocked)
            {
                if (attackKeyHold)
                    ChargingProjectileShoot();
                else if (Input.GetKeyUp(attackKey) || controllerMobileAttackButtonUp)
                    if (isChargingProjectileAttack)
                        ReleaseChargeProjectileShoot();
                    else
                    {
                        ShortAttack();
                    }
            }
            else if (Input.GetKeyDown(attackKey)/* || attackOnController == 2*/ || mobileAttackButtonDown)
            {
                ShortAttack();
            }
        }
        else if (attackKeyHold)
            ChargingBurstAttack();
        else if (Input.GetKeyUp(attackKey) || controllerMobileAttackButtonUp)
            if (isChargingBurstAttack)
                ReleaseChargeBurstAttack();
            else
            {
                ResetShortAttackCombo();
                ShortAttack();
            }

        if (unlockableAbilities[0].isUnlocked)
            if (Input.GetKeyDown(dashKey) || DashOnController == 2 || mobileDashButtonDown)
                StartCoroutine(Dash());
    }

    void CheckForAttackHoldKey()
    {
        if (Input.GetKey(attackKey) || attackOnController == 1 || mobileAttackButtonDown)
        {
            attackKeyHoldTime += Time.deltaTime;
            if (attackKeyHoldTime >= ATTACK_KEY_HOLD_TRIGGER_THRESHOLD)
                attackKeyHold = true;
        }
        else if (attackKeyHold || attackOnController == 0 || !mobileAttackButtonDown)
            ResetAttackHoldKey();
    }

    void ResetAttackHoldKey()
    {
        attackKeyHoldTime = 0f;
        attackKeyHold = false;
    }

    #region Mobile and Controller Input
    public void MobileAttackButton()
    {
        mobileAttackButtonDown = true;
    }

    public void MobileAttackButtonUp()
    {
        if (mobileAttackButtonDown)
            StartCoroutine(AttackMobileButtonHoldRelease());
        mobileAttackButtonDown = false;
    }

    IEnumerator AttackMobileButtonHoldRelease()
    {
        controllerMobileAttackButtonUp = true;
        yield return new WaitForSeconds(0.001f);
        controllerMobileAttackButtonUp = false;
    }

    public void DashMobileButton()
    {
        StartCoroutine(ReleaseDashMobileButton());
    }
    IEnumerator ReleaseDashMobileButton()
    {
        mobileDashButtonDown = true;
        yield return new WaitForSeconds(0.001f);
        mobileDashButtonDown = false;
    }

    void ControllerInputs()
    {
        if (Input.GetAxisRaw("Attack") != 0)
        {
            if (attackOnController == 0)
                StartCoroutine(ReleaseAttackController());
        }
        else if (Input.GetAxisRaw("Attack") == 0)
            AttackControllerRelease();

        if (Input.GetAxisRaw("Dash") != 0)
        {
            if (DashOnController == 0)
                StartCoroutine(ReleaseDashController());
        }
        else if (Input.GetAxisRaw("Dash") == 0)
            DashOnController = 0;
    }

    void AttackControllerRelease()
    {
        if (attackOnController == 1)
            StartCoroutine(AttackControllerButtonHoldRelease());

        attackOnController = 0;
    }

    IEnumerator ReleaseAttackController()
    {
        attackOnController = 2;
        yield return new WaitForSeconds(0.001f);
        attackOnController = 1;

    }

    IEnumerator AttackControllerButtonHoldRelease()
    {
        controllerMobileAttackButtonUp = true;
        yield return new WaitForSeconds(0.001f);
        controllerMobileAttackButtonUp = false;
    }

    IEnumerator ReleaseDashController()
    {
        DashOnController = 2;
        yield return new WaitForSeconds(0.1f);
        DashOnController = 1;

    }
    #endregion

    void FixedUpdate()
    {
        if (canControl)
            HandleMovement();
    }

    void HandleMovement()
    {
        if (isDashing || ShortAttackComboNo >= 1 || IsCharging || IsReleasing)
            return;
        EndShortAttackCombo();

        bool isMoving = moveInput.x != 0 || moveInput.y != 0;
        var moveVelocity = Vector3.zero;
        if (isMoving)
        {
            moveVelocity = new Vector3(moveInput.x * moveSpeedX, 0f, moveInput.y * moveSpeedZ);
            rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
            lastMoveDir = new Vector3(moveInput.x, 0f, moveInput.y);

            UpdateFaceDir(moveInput.x);
            UpdateRaycastOrigins();
        }
        Animator.SetBool("isMoving", moveVelocity != Vector3.zero);
    }

    #region Dashing
    IEnumerator Dash()
    {
        PlayParticle(dashParticleAnimator, dashParticlePos, "Dash Particle");
        Animator.SetBool("isDashing", isDashing = true);

        EndShortAttackCombo();
        ResetChargingAndReleasing();

        // Add Feedbacks
        gameCam.Teleport();

        yield return new WaitForSeconds(.2f);

        TakeDamage(dashCost, false, false);

        UpdateRaycastOrigins();

        float dashDistance = (dashSpeed * dashDuration) + .5f;
        Vector3 endPos = transform.position + (lastMoveDir * dashDistance);
        if (lastMoveDir.x != 0)
            CheckDashObstacles(rayCountX, lastMoveDir.x, raySpacingX,
                               dashDistance, ref endPos.x, boxCollider.size.x, true);
        if (lastMoveDir.z != 0)
            CheckDashObstacles(rayCountZ, lastMoveDir.z, raySpacingZ,
                               dashDistance, ref endPos.z, boxCollider.size.z, false);

        var startTime = Time.time;
        while ((Time.time - startTime) < dashDuration)
        {
            if (!isDashing)
            {
                transform.position = new Vector3(endPos.x, transform.position.y, endPos.z);

                yield return new WaitForSeconds(dashDuration - (Time.time - startTime));
                break;
            }
            else
                transform.position += lastMoveDir * dashSpeed * Time.deltaTime;
            yield return null;
        }

        Animator.SetBool("isDashing", isDashing = false);
    }

    void CheckDashObstacles(float rayCountAxis, float lastMoveDirAxis, float raySpacingAxis,
                            float dashDistance, ref float endPosAxis, float colliderSizeAxis, bool checkX)
    {
        for (int offset = 0; offset < rayCountAxis; offset++)
        {
            Vector3 rayOrigin = lastMoveDirAxis == 1 ? (checkX ? raycastOrigins.frontRight
                                                               : raycastOrigins.backLeft)
                                                     : raycastOrigins.frontLeft;
            rayOrigin += (checkX ? Vector3.forward : Vector3.right) * (raySpacingAxis * offset);
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, lastMoveDir, out hit, dashDistance, collisionMask))
            {
                endPosAxis = (checkX ? hit.point.x
                                     : hit.point.z) - (colliderSizeAxis / 2) * lastMoveDirAxis;
                isDashing = false;
            }
            Debug.DrawRay(rayOrigin, lastMoveDir * dashDistance, Color.red, 1f);
        }
    }

    public void EndDash()
    {
        Animator.SetBool("isDashing", isDashing = false);
    }
    #endregion

    void ShortAttack()
    {
        if (IsCharging || IsReleasing)
            return;

        lastShortAttackTime = Time.time;
        ShortAttackComboNo++;
        if (ShortAttackComboNo == 1)
        {
            Animator.SetInteger("shortAttackCombo", 1);
            PlayParticle(shortAttack1ParticleAnimator, shortAttackParticlePos,
                         "Short Attack 1 Particle");
        }
        ShortAttackComboNo = Mathf.Clamp(ShortAttackComboNo, 0, unlockableAbilities[1].isUnlocked ? 3 : 2);
    }

    void ChargingBurstAttack()
    {
        if (IsReleasing)
            return;

        if (!isChargingBurstAttack)
            Animator.SetBool("isChargingBurstAttack", isChargingBurstAttack = true);

        if (burstAttackBox.Damage != burstAttackDamages[burstAttackDamages.Length - 1])
        {
            burstAttackChargeTime += burstAttackChargeSpeed * Time.deltaTime;
            for (int i = 0; i < burstAttackDamages.Length; i++)
                if (burstAttackChargeTime >= burstAttackDamageIncThreshold * i)
                    if (burstAttackBox.Damage != burstAttackDamages[i])
                    {
                        PlayParticle(chargeParticleAnimator, chargeParticlePos, "Charge Particle");
                        burstAttackBox.Damage = burstAttackDamages[i];
                    }
        }
    }

    void ReleaseChargeBurstAttack()
    {
        Animator.SetBool("isReleasingBurstAttack", isReleasingBurstAttack = true);
        PlayParticle(burstAttackReleaseParticleAnimator, burstAttackReleaseParticlePos,
                     "Burst Attack Release Particle");
        TakeDamage(burstChargeAttackCost, false, false);
    }

    public void TryNextShortAttackCombo(int nextComboNo)
    {
        if (ShortAttackComboNo >= nextComboNo)
        {
            Animator.SetInteger("shortAttackCombo", nextComboNo);
            if (nextComboNo == 2)
                PlayParticle(shortAttack2ParticleAnimator, shortAttackParticlePos,
                             "Short Attack 2 Particle");
        }
        else
            ResetShortAttackCombo();
    }

    public void EndShortAttackCombo()
    {
        ResetShortAttackCombo();
        Animator.SetBool("isChargingBurstAttack", isChargingBurstAttack = false);
        Animator.SetBool("isReleasingBurstAttack", isReleasingBurstAttack = false);
        burstAttackChargeTime = 0f;
        burstAttackBox.Damage = 0;
    }

    public void ResetShortAttackCombo()
    {
        Animator.SetInteger("shortAttackCombo", ShortAttackComboNo = 0);
    }

    void ResetChargingAndReleasing()
    {
        Animator.SetBool("isChargingBurstAttack", isChargingBurstAttack = false);
        Animator.SetBool("isChargingProjectileAttack", isChargingProjectileAttack = false);

        Animator.SetBool("isReleasingBurstAttack", isReleasingBurstAttack = false);
        Animator.SetBool("isReleasingProjectileAttack", isReleasingProjectileAttack = false);
    }

    void ChargingProjectileShoot()
    {
        if (isChargingBurstAttack || isDashing)
            return;

        if (ShortAttackComboNo != 0)
            ResetShortAttackCombo();

        if (!isChargingProjectileAttack)
            Animator.SetBool("isChargingProjectileAttack", isChargingProjectileAttack = true);

        if (projectileDamage != projectileAttackDamages[projectileAttackDamages.Length - 1])
        {
            projectileAttackChargeTime += projectileChargeSpeed * Time.deltaTime;
            for (int i = 0; i < projectileAttackDamages.Length; i++)
                if (projectileAttackChargeTime >= projectileAttackDamageIncThreshold * i)
                    if (projectileDamage != projectileAttackDamages[i])
                    {
                        PlayParticle(chargeParticleAnimator, chargeParticlePos, "Charge Particle");
                        projectileDamage = projectileAttackDamages[i];
                    }
        }
    }

    void ReleaseChargeProjectileShoot()
    {
        if (isChargingBurstAttack)
            return;

        Animator.SetBool("isReleasingProjectileAttack", isReleasingProjectileAttack = true);
    }

    public void ShootProjectile()
    {
        int faceDir = flipper.rotation.y == 0f ? 1 : -1;
        ObjectPooler
            .GetProjectile(projectilePrefab.id, LocalisePos(projectileSpawnPoint), flipper.rotation,
                           new ObjectData(projectileDamage, projectileMoveSpeed, faceDir, true));
        TakeDamage(projectileChargeAttackCost, false, false);
    }

    void EndProjectileShoot()
    {
        Animator.SetBool("isChargingProjectileAttack", isChargingProjectileAttack = false);
        Animator.SetBool("isReleasingProjectileAttack", isReleasingProjectileAttack = false);
        projectileAttackChargeTime = 0f;
        projectileDamage = 0;
    }

    void PlayParticle(Animator particleAnimator, Vector3 particlePos, string animationName)
    {
        particleAnimator.transform.position = LocalisePos(particlePos);
        particleAnimator.transform.rotation = flipper.rotation;
        particleAnimator.Play(animationName);
    }

    public bool TryUnlockAbility(int itemNo)
    {
        return TryUnlockUnlockable(ref unlockableAbilities[itemNo]);
    }

    public bool TryUnlockBuff(int itemNo)
    {
        if (!TryUnlockUnlockable(ref unlockableBuffs[itemNo].unlockable))
            return false;

        unlockableBuffs[itemNo].incLevelCurrent++;

        if (itemNo == 0)
            UpgradeBloodCapacity();
        else
            UpgradeBloodSteal();
        return true;
    }

    bool TryUnlockUnlockable(ref Unlockable unlockable)
    {
        int unlockCost = unlockable.unlockCost;
        if (CurrentHealth >= unlockCost)
        {
            TakeDamage(unlockCost, true, false);
            return unlockable.isUnlocked = true;
        }
        return false;
    }

    void UpgradeBloodCapacity()
    {
        maxHealth += bloodCapacityIncPercent;
        healthBar.UpgradeCapacity(bloodCapacityIncPercent);
    }

    void UpgradeBloodSteal()
    {
    }

    Bounds bounds;
    public void UpdateRaycastOrigins()
    {
        bounds = boxCollider.bounds;
        // MAKE SPACE FOR RAYCASTS BY SHRINKING BOUNDS BY SKIN WIDTH TO FIRE FROM INSIDE THE SQUARE IN CASE
        // THE SQUARE'S EDGES ARE BLOCKED BY AN OBSTACLE
        bounds.Expand(SKIN_WIDTH * -2);

        // SET RAYCAST ORIGINS BY BOUND CORNERS
        raycastOrigins.backLeft = new Vector3(bounds.min.x, bounds.center.y, bounds.max.z);
        raycastOrigins.backRight = new Vector3(bounds.max.x, bounds.center.y, bounds.max.z);
        raycastOrigins.frontLeft = new Vector3(bounds.min.x, bounds.center.y, bounds.min.z);
        raycastOrigins.frontRight = new Vector3(bounds.max.x, bounds.center.y, bounds.min.z);
    }

    public void CalculateRaySpacing()
    {
        bounds = boxCollider.bounds;
        bounds.Expand(SKIN_WIDTH * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        // HOW MANY RAYS FIT IN THE WIDTH/HEIGHT OF PLAYER
        rayCountX = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        rayCountZ = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

        // CALCULATE SPACING: SHRINKED BOUNDS X||Y EDGE / (HORIZONTAL||VERTICAL RAY COUNT - 1)
        raySpacingX = bounds.size.y / (rayCountX - 1);
        raySpacingZ = bounds.size.x / (rayCountZ - 1);
    }

    public void GainHealth(int gainedHealth)
    {
        if (CurrentHealth == maxHealth)
            return;
        int bonus = (int)(gainedHealth * (unlockableBuffs[1].incPercent / 100f) * unlockableBuffs[1].incLevelCurrent);
        gainedHealth += bonus;
        if (CurrentHealth + gainedHealth > maxHealth)
            gainedHealth = maxHealth - CurrentHealth;
        healthBar.SetHealth(CurrentHealth += gainedHealth);

        SpawnCombatText(gainedHealth, combatTextGainedColour, false);
    }

    public override void TakeDamage(int damage, bool forceDamage = false, bool playHurtFX = true)
    {
        if (canLoseHealth || forceDamage)
        {
            base.TakeDamage(damage);

            if (!playHurtFX)
                return;

            if (damage > 1 && !isDashing)
            {
                StartCoroutine(InvicibilityFrames());
                DamageFeedback.PlayFeedbacks();
                AudioStation.StartNewRandomSFXPlayer(HurtSounds);
            }
        }
    }

    public IEnumerator InvicibilityFrames()
    {
        canLoseHealth = false;
        yield return new WaitForSeconds(.5f);
        canLoseHealth = true;
    }

    public override void Die()
    {
        gameManager.GameOver();
        AudioStation.StartNewSFXPlayer(DieSound);
        var audioListenerObject = new GameObject("Audio Listener Object");
        audioListenerObject.transform.position = transform.position;
        audioListenerObject.AddComponent<AudioListener>();
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle") || other.transform.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<EnemyMovement>() && isDashing)
                return;
            rb.velocity = Vector3.zero;
            isDashing = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(raycastOrigins.backLeft, .1f);
        Gizmos.DrawSphere(raycastOrigins.backRight, .1f);
        Gizmos.DrawSphere(raycastOrigins.frontLeft, .1f);
        Gizmos.DrawSphere(raycastOrigins.frontRight, .1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(LocalisePos(projectileSpawnPoint), .2f);
    }
    public void WalkSound()
    {
        AudioStation.StartNewRandomSFXPlayer(WalkSounds, transform.position, .9f, 1.1f);
    }
    public void PunchSound()
    {
        AudioStation.StartNewRandomSFXPlayer(PunchSounds, transform.position, .9f, 1.1f);
    }
    public void DashSound()
    {
        AudioStation.StartNewSFXPlayer(DashSounds[0], transform.position, .9f, 1.1f);
    }
    public void WingFlap()
    {
        AudioStation.StartNewSFXPlayer(DashSounds[1], transform.position, .9f, 1.1f);
    }
    public void ChargingSound()
    {
        AudioStation.StartNewRandomSFXPlayer(ChargingSounds, transform.position, .9f, 1.1f);
    }
    public void BurstSound()
    {
        AudioStation.StartNewRandomSFXPlayer(BurstSounds, transform.position, .9f, 1.1f);
    }
}