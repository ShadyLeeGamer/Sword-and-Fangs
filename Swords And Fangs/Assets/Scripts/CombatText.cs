using UnityEngine;
using TMPro;

public class CombatText : MonoBehaviour, IPooledObject
{
    public int id;

    TextMeshPro text;

    ObjectPooler objectPooler;
    
    void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    public void Initialise(ObjectData combatTextData)
    {
        text.color = combatTextData.colour;
        text.text = combatTextData.text;
    }

    public void EndCombatText()
    {
        objectPooler.RecycleCombatText(this);
    }
}