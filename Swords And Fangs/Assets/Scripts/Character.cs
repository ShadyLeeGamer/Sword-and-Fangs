using UnityEngine;
using TMPro;

public abstract class Character : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth;
    [SerializeField] Color combatTextDamageColour;
    public int CurrentHealth { get; set; }

    [HideInInspector] public Transform flipper, looker;

    public HealthBar healthBar;
    public CombatTextSettings combatTextSettings;

    [HideInInspector] public GameCamera gameCam;

    public ObjectPooler ObjectPooler { get; private set; }

    public Animator Animator { get; private set; }

    public AudioStation AudioStation { get; private set; }

    public virtual void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public virtual void Start()
    {
        flipper = transform.GetChild(0);
        looker = flipper.GetChild(0);

        gameCam = GameCamera.Instance;
        ObjectPooler = ObjectPooler.Instance;
        AudioStation = AudioStation.Instance;
    }

    public virtual void Update()
    {
        //looker.LookAt(gameCam.transform);
    }

    public void UpdateFaceDir(float moveDelta)
    {
        float flipAngle = flipper.eulerAngles.y;
        if (moveDelta > 0f)
            flipAngle = 0f;
        else if (moveDelta < 0f)
            flipAngle = 180f;
        flipper.eulerAngles = new Vector3(0f, flipAngle, 0f);
    }

    public Vector3 LocalisePos(Vector3 pos)
    {
        return transform.position + LocaliseFlipPos(pos);
    }

    public Vector3 LocaliseFlipPos(Vector3 pos)
    {
        return new Vector3(pos.x * (flipper.rotation.y == 0f ? 1f : -1f),
                           pos.y,
                           pos.z);
    }

    public virtual void TakeDamage(int damage, bool forceDamage = false, bool playHurtFX = true)
    {
        healthBar.SetHealth(CurrentHealth -= damage);
        SpawnCombatText(damage, combatTextDamageColour, true);

        if (CurrentHealth <= 0)
            Die();
    }

    public void SpawnCombatText(int healthDelta, Color textColour, bool isDamage)
    {
        CombatText combatTextPrefab = isDamage ? combatTextSettings.damagePrefab
                                                : combatTextSettings.gainedPrefab;
        Vector3 combatTextPos = healthBar.transform.position;
        combatTextPos.z = combatTextPos.z - .1f;
        CombatText newCombatText =
            ObjectPooler.GetCombatText(combatTextPrefab.id, combatTextPos, Quaternion.identity,
                new ObjectData(textColour, (isDamage ? "-" : "+") + healthDelta.ToString()));
        combatTextSettings.AdjustSize(newCombatText, healthDelta);
    }

    public abstract void Die();
}