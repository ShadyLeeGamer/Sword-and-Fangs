using UnityEngine;
using TMPro;

public abstract class Interactable : MonoBehaviour
{
    public TextMeshPro interactTXT;

    public DraculaController dracula;
    
    public bool inRange;

    public Collider InteractBox { get; private set; }

    public virtual void Awake()
    {
        InteractBox = GetComponent<Collider>();
    }

    public void Initialise(DraculaController dracula)
    {
        this.dracula = dracula;
    }

    public virtual void Update()
    {
        if (inRange)
            if (dracula.ShortAttackComboNo > 0)
                Interact();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DraculaController>())
            UpdateRange(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DraculaController>())
            UpdateRange(false);
    }

    public virtual void Interact()
    {
        interactTXT.gameObject.SetActive(false);
    }

    public virtual void UpdateRange(bool value)
    {
        interactTXT.gameObject.SetActive(inRange = value);
    }
}