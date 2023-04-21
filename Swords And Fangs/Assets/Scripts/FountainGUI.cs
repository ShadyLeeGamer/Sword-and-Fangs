using UnityEngine;
using UnityEngine.UI;

public class FountainGUI : MonoBehaviour
{
    [SerializeField] HealthBar draculaBloodBar;

    [System.Serializable]
    public struct Item
    {
        public Button Btn;
        public HealthBar bloodCostBar;
        public HealthBar EntireBar;
        public Image unlockedImg;
    }
    [SerializeField] Item[] abilityItems, buffItems;

    [SerializeField] Slider[] buffLevelBars;

    DraculaController dracula;

    public void UpdateGUI()
    {
        if (!dracula)
        {
            dracula = DraculaController.Instance;
            UpdateBloodBarsMax();
        }
        UpdateBloodBars();
        UpdateBuffLevelBars();
    }

    void UpdateBloodBarsMax()
    {
        draculaBloodBar.SetMaxHealth(dracula.maxHealth);
        for (int i = 0; i < abilityItems.Length; i++)
        {
            abilityItems[i].bloodCostBar.SetMaxHealth(dracula.maxHealth);
            abilityItems[i].EntireBar.SetMaxHealth(dracula.maxHealth);
        }
        for (int i = 0; i < buffItems.Length; i++)
        {
            buffItems[i].bloodCostBar.SetMaxHealth(dracula.maxHealth);
            buffItems[i].EntireBar.SetMaxHealth(dracula.maxHealth);
        }

        for (int i = 0; i < buffLevelBars.Length; i++)
            buffLevelBars[i].maxValue = dracula.unlockableBuffs[i].incLevelMax;
    }

    void UpdateBloodBars()
    {
        draculaBloodBar.SetHealth(dracula.CurrentHealth);
        for (int i = 0; i < abilityItems.Length; i++)
            UpdateAbilityBloodCostBar(i);
        for (int i = 0; i < buffItems.Length; i++)
            UpdateBuffBloodCostBar(i);
    }

    void UpdateAbilityBloodCostBar(int itemNo)
    {
        abilityItems[itemNo].bloodCostBar.SetHealth(
            dracula.CurrentHealth - dracula.unlockableAbilities[itemNo].unlockCost);
        abilityItems[itemNo].EntireBar.SetHealth(
            dracula.CurrentHealth);
    }

    void UpdateBuffBloodCostBar(int itemNo)
    {
        buffItems[itemNo].bloodCostBar.SetHealth(
            dracula.CurrentHealth - dracula.unlockableBuffs[itemNo].unlockable.unlockCost);
        buffItems[itemNo].EntireBar.SetHealth(
            dracula.CurrentHealth);
    }

    void UpdateBuffLevelBars()
    {
        for (int i = 0; i < buffLevelBars.Length; i++)
            UpdateBuffLevelBar(i);
    }

    void UpdateBuffLevelBar(int buffNo)
    {
        buffLevelBars[buffNo].value = dracula.unlockableBuffs[buffNo].incLevelCurrent;
    }

    public void BuyAbility(int itemNo)
    {
        if (dracula.TryUnlockAbility(itemNo))
        {
            UpdateBloodBars();
            DeactivateItemBTN(abilityItems[itemNo]);
        }
        else
        {
            // NOT ENOUGH BLOOD
        }
    }

    public void BuyBuff(int itemNo)
    {
        if (dracula.TryUnlockBuff(itemNo))
        {
            UpdateBloodBarsMax();
            UpdateBloodBars();
            UpdateBuffLevelBar(itemNo);
            if (dracula.unlockableBuffs[itemNo].IncLevelIsAtMax)
                DeactivateItemBTN(buffItems[itemNo]);
        }
        else
        {
            // NOT ENOUGH BLOOD
        }
    }

    void DeactivateItemBTN(Item item)
    {
        item.bloodCostBar.gameObject.SetActive(false);
        item.unlockedImg.gameObject.SetActive(true);
        Destroy(item.Btn);
    }
}