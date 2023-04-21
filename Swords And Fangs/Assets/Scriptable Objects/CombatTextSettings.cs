using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "CombatTextSettings")]
public class CombatTextSettings : ScriptableObject
{
    public CombatText damagePrefab, gainedPrefab;
    public float sizeMin, sizeMax;

    public void AdjustSize(CombatText combatText, int healthDelta)
    {
        float interpolator =  healthDelta / 100f;
        combatText.transform.localScale = Vector3.one * Mathf.Lerp(sizeMin, sizeMax, interpolator);
    }
}