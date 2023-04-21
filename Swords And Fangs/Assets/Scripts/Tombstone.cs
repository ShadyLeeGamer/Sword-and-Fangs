using UnityEngine;

public class Tombstone : Interactable
{
    [SerializeField] DraculaController draculaPrefab;
    [SerializeField] Vector3 spawnOffset;

    public static Tombstone Instance { get; private set; }

    GameManager gameManager;

   public override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    public DraculaController SummonDracula()
    {
        return Instantiate(draculaPrefab, transform.position + spawnOffset, Quaternion.identity);
    }

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    public override void Interact()
    {
        base.Interact();
        gameManager.StartNextWave();
    }

    public override void UpdateRange(bool value)
    {
        base.UpdateRange(value);
        if (value)
            interactTXT.text = Random.value <= 0.1f ? "Next Knight [Attack]" : "Next Night [Attack]";
    }
}