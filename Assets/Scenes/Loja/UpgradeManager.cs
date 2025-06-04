using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    private Dictionary<string, bool> unlockedUpgrades = new Dictionary<string, bool>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockUpgrade(string upgradeID)
    {
        if (!unlockedUpgrades.ContainsKey(upgradeID))
        {
            unlockedUpgrades.Add(upgradeID, true);
            PlayerPrefs.SetInt("upgrade_" + upgradeID, 1);
        }
    }

    public bool HasUpgrade(string upgradeID)
    {
        return unlockedUpgrades.ContainsKey(upgradeID) && unlockedUpgrades[upgradeID];
    }

    void LoadUpgrades()
    {
        string[] allUpgrades = {
            "hiloMultiplicador",
            "hiloSegundaChance",
            "rodaApostaReduzida",
            "rodaSempreGanha",
            "slotMaisCombos",
            "slotJackpotGeneroso",
            "battleDamageBoost",
            "battleDefenseBoost"
        };

        foreach (string id in allUpgrades)
        {
            if (PlayerPrefs.GetInt("upgrade_" + id, 0) == 1)
            {
                unlockedUpgrades[id] = true;
            }
        }
    }

    public void ResetUpgrades()
    {
        string[] allUpgrades = {
            "hiloMultiplicador",
            "hiloSegundaChance",
            "rodaApostaReduzida",
            "rodaSempreGanha",
            "slotMaisCombos",
            "slotJackpotGeneroso",
            "battleDamageBoost",
            "battleDefenseBoost"
        };

        foreach (string id in allUpgrades)
        {
            PlayerPrefs.DeleteKey("upgrade_" + id);
        }

        unlockedUpgrades.Clear();
    }
}
